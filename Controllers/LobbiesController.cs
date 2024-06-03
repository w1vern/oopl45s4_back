using MafiaAPI.Hub;
using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbiesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<SignalRHub> _hubContext;

        public LobbiesController(IMatchRepository matchRepository, IPlayerStateRepository playerStateRepository, IUserRepository userRepository, IHubContext<SignalRHub> hubContext)
        {
            _matchRepository = matchRepository;
            _playerStateRepository = playerStateRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        [Authorize]
        [HttpPost("create", Name = "CreateMatch")]
        public async Task<IActionResult> CreateMatch()
        {
            string? userRequestingId = User.Identity.Name;
            var userHost = await _userRepository.Get(userRequestingId);
            var matchesList = userHost.PlayerStates;
            foreach (var mch in matchesList)
            {
                if (mch.Match.MatchEnd == null && mch.Role == "Host")
                {
                    return BadRequest();
                }
            }
            Match match = new();
            await _matchRepository.Create(match);
            PlayerState playerState = new()
            {
                Role = "Host",
                IsAlive = true,
                Match = match,
                User = userHost
            };
            await _playerStateRepository.Create(playerState);
            return Ok(match.Id);
        }

        [Authorize]
        [HttpPost("connect/{id}", Name = "ConnectMatch")]
        public async Task<IActionResult> ConnectMatch(string id)
        {
            var match = await _matchRepository.Get(id);
            if(match == null || match.MatchStart != null)
            {
                return BadRequest();
            }
            string? userRequestingId = User.Identity.Name;
            var userRequesting = await _userRepository.Get(userRequestingId);
            foreach (var player in match.PlayerStates)
            {
                if(player.UserId == userRequestingId)
                {
                    return BadRequest();
                }
            }
            PlayerState playerState = new()
            {
                Role = "Player",
                IsAlive = true,
                Match = match,
                User = userRequesting
            };
            await _playerStateRepository.Create(playerState);
            await _hubContext.Clients.Group(id).SendAsync("Refresh");
            return Ok();
        }
    }
}
