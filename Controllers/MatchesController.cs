using MafiaAPI.Hub;
using MafiaAPI.Models;
using MafiaAPI.Repositories;
using MafiaAPI.RequestModels;
using MafiaAPI.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHubContext<SignalRHub> _hubContext;

        public MatchesController(IMatchRepository matchRepository, IPlayerStateRepository playerStateRepository, IRoleRepository roleRepository, IHubContext<SignalRHub> hubContext)
        {
            _matchRepository = matchRepository;
            _playerStateRepository = playerStateRepository;
            _roleRepository = roleRepository;
            _hubContext = hubContext;
        }

        [HttpGet("/test")]
        public IActionResult TestMethod()
        {
            return Ok(new StartMatchRequest() { Roles = [new RoleInfo() { Count=1, Id="asfd"}] });
        }

        [Authorize]
        [HttpPost("{id:guid}/start", Name = "StartMatch")]
        public async Task<IActionResult> StartMatch([FromBody] List<RoleInfo> startMatchRequest, string id)
        {
            string? userRequestingId = User.Identity.Name;
            Match match = await _matchRepository.Get(id);
            foreach (var ps in match.PlayerStates.ToList())
            {
                if (ps.Role.Name == "Host")
                {
                    if(ps.User.Id != userRequestingId)
                    {
                        return Forbid();
                    }
                    break;
                }
            }
            if (match == null)
            {
                return BadRequest();
            }
            if (match.MatchStart != null)
            {
                return Conflict();
            }
            List<string> roles = [];
            foreach (var item in startMatchRequest)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    roles.Add(item.Id);
                }
            }
            var playerStates = match.PlayerStates.ToList();
            var civilian_id = (await _roleRepository.GetByName("Citizen")).Id;
            while (roles.Count < playerStates.Count - 1) {
                roles.Add(civilian_id);
            }
            var roles_arr = roles.ToArray();
            Random.Shared.Shuffle(roles_arr);


            int role_ind = 0;
            for (int i = 0; i < playerStates.Count; i++)
            {
                if (playerStates[i].RoleId == null)
                {
                    playerStates[i].RoleId = roles_arr[role_ind];
                    await _playerStateRepository.Update(playerStates[i]);
                    role_ind++;
                }  
            }

            match.MatchStart = DateTime.Now;
            await _matchRepository.Update(match);

            await _hubContext.Clients.Group(id).SendAsync("Refresh");
            return Ok();
        }

        [Authorize]
        [HttpGet("{id:guid}", Name = "GetMatchInfo")]
        public async Task<IActionResult> GetInfo(string id)
        {
            var match = await _matchRepository.Get(id);
            if (match == null)
            {
                return BadRequest();
            }
            if(!match.PlayerStates.Any(x => x.User.Id == User.Identity.Name)) {
                return BadRequest();
            }
            MatchRequest matchRequest = new()
            {
                Id = match.Id,
                MatchStart = match.MatchStart,
                MatchEnd = match.MatchEnd,
                MatchResult = match.MatchResult
            };
            foreach (var ps in match.PlayerStates.ToList())
            {
                matchRequest.PlayersIds.Add(ps.User.Id);
            }
            return Ok(matchRequest);
        }

        [Authorize]
        [HttpGet("available", Name = "GetAvailableMatches")]
        public IActionResult GetAvailableMatches()
        {
            List<MatchRequest> matchesAvailable = [];
            var matches = _matchRepository.Get().ToList();
            foreach (var match in matches)
            {
                if (match.MatchStart == null)
                {
                    MatchRequest matchRequest = new()
                    {
                        Id = match.Id,
                        MatchStart = match.MatchStart,
                        MatchEnd = match.MatchEnd,
                        MatchResult = match.MatchResult
                    };
                    foreach (var ps in match.PlayerStates.ToList())
                    {
                        matchRequest.PlayersIds.Add(ps.User.Id);
                    }
                    matchesAvailable.Add(matchRequest);
                }
            }
            return Ok(matchesAvailable);
        }

        [Authorize]
        [HttpPost("{id:guid}/kill")]
        public async Task<IActionResult> KillInMatch(string id, [FromBody] string playerId)
        {
            var match = await _matchRepository.Get(id);
            var ps = match.PlayerStates.FirstOrDefault(x => x.UserId == playerId);
            ps.IsAlive = false;
            await _playerStateRepository.Update(ps);
            await _hubContext.Clients.Group(id).SendAsync("Refresh");
            return Ok();
        }

        [Authorize]
        [HttpPost("{id:guid}/revive")]
        public async Task<IActionResult> ReviveInMatch(string id, [FromBody] string playerId)
        {
            var match = await _matchRepository.Get(id);
            var ps = match.PlayerStates.FirstOrDefault(x => x.UserId == playerId);
            ps.IsAlive = true;
            await _playerStateRepository.Update(ps);
            await _hubContext.Clients.Group(id).SendAsync("Refresh");
            return Ok();
        }

        [Authorize]
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetRolesInMatch(string id)
        {
            List<PlayersRoleRequest> playersRoles = [];
            var match = await _matchRepository.Get(id);
            foreach (var ps in match.PlayerStates.ToList())
            {
                playersRoles.Add(new()
                {
                    PlayerId = ps.UserId,
                    RoleName = ps.Role?.Name,
                    IsAlive = ps.IsAlive
                });
            }
            return Ok(playersRoles);
        }

        [Authorize]
        [HttpGet("{id:guid}/get_state")]
        public async Task<IActionResult> GetState(string id)
        {
            var match = await _matchRepository.Get(id);
            if (match.MatchStart == null)
            {
                return BadRequest();
            }
            return Ok(match.currentState);
        }

        [Authorize]
        [HttpPost("{id:guid}/switch_state")]
        public async Task<IActionResult> SwitchState(string id)
        {
            var match = await _matchRepository.Get(id);
            if(match.MatchStart == null)
            {
                return BadRequest();
            }
            HashSet<int> states = [0];
            foreach (var ps in match.PlayerStates.ToList())
            {
                states.Add(ps.Role.Priority);
            }
            var sortedStates = states.ToList();
            sortedStates.Sort();
            match.currentState = sortedStates.IndexOf(match.currentState) == sortedStates.Count - 1 ? 0 : match.currentState + 1;
            await _matchRepository.Update(match);
            await _hubContext.Clients.Group(id).SendAsync("Refresh");
            return Ok(match.currentState);
        }
    }
}
