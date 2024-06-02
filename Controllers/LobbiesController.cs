using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbiesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IUserRepository _userRepository;

        public LobbiesController(IMatchRepository matchRepository, IPlayerStateRepository playerStateRepository, IUserRepository userRepository)
        {
            _matchRepository = matchRepository;
            _playerStateRepository = playerStateRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost("create", Name = "CreateMatch")]
        public async Task<IActionResult> CreateMatch()
        {
            string? userRequestingId = User.Identity.Name;
            var userHost = await _userRepository.Get(userRequestingId);
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
            return Ok();
        }
    }
}
