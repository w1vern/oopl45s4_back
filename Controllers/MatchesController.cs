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
        private readonly IHubContext<SignalRHub> _hubContext;

        public MatchesController(IMatchRepository matchRepository, IPlayerStateRepository playerStateRepository, IHubContext<SignalRHub> hubContext)
        {
            _matchRepository = matchRepository;
            _playerStateRepository = playerStateRepository;
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost("start", Name = "StartMatch")]
        public async Task<IActionResult> StartMatch([FromBody] string id)
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

            match.MatchStart = DateTime.Now;
            await _matchRepository.Update(match);
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
        [HttpPost("{id}/kill")]
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
        [HttpPost("{id}/revive")]
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
        [HttpPost("{id}/roles")]
        public async Task<IActionResult> GetRolesInMatch(string id)
        {
            List<PlayersRoleRequest> playersRoles = [];
            var match = await _matchRepository.Get(id);
            foreach (var ps in match.PlayerStates.ToList())
            {
                playersRoles.Add(new()
                {
                    PlayerId = ps.UserId,
                    RoleName = ps.Role.Name
                });
            }
            return Ok(playersRoles);
        }
    }
}
