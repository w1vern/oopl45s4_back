using MafiaAPI.Hub;
using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbiesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerStateRepository _playerStateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHubContext<SignalRHub> _hubContext;

        public LobbiesController(IMatchRepository matchRepository, IPlayerStateRepository playerStateRepository, 
            IUserRepository userRepository, IHubContext<SignalRHub> hubContext, IRoleRepository roleRepository)
        {
            _matchRepository = matchRepository;
            _playerStateRepository = playerStateRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _roleRepository = roleRepository;
        }

        [Authorize]
        [HttpPost("create", Name = "CreateMatch")]
        [SwaggerOperation(Summary = "Создает новый матч (лобби)", Description = "Возвращает GUID созданного матча")]
        [SwaggerResponse(200, Description = "Возвращает ID созданного матча", Type = typeof(string))]
        [SwaggerResponse(400, Description = "Матч в процессе для данного хоста уже существует")]
        public async Task<IActionResult> CreateMatch()
        {
            string? userRequestingId = User.Identity.Name;
            var userHost = await _userRepository.Get(userRequestingId);
            var matchesList = userHost.PlayerStates.ToList();
            foreach (var mch in matchesList)
            {
                if (mch.Match.MatchEnd == null && mch.Role.Name == "Host")
                {
                    return BadRequest();
                }
            }
            Match match = new();
            await _matchRepository.Create(match);
            Role roleHost = await _roleRepository.GetByName("Host");
            PlayerState playerState = new()
            {
                Role = roleHost,
                IsAlive = true,
                Match = match,
                User = userHost
            };
            await _playerStateRepository.Create(playerState);
            return Ok(match.Id);
        }

        [Authorize]
        [HttpPost("connect/{id:guid}", Name = "ConnectMatch")]
        [SwaggerOperation(Summary = "Подключение к матчу (лобби)", Description = "Принимает из query уникальный идентификатор лобби и подключает игрока")]
        [SwaggerResponse(200, Description = "Успешное подключение игрока к лобби")]
        [SwaggerResponse(400, Description = "Матч не существует / в процессе или игрок уже подключен")]
        public async Task<IActionResult> ConnectMatch(string id)
        {
            var match = await _matchRepository.Get(id);
            if(match == null || match.MatchStart != null)
            {
                return BadRequest();
            }
            string? userRequestingId = User.Identity.Name;
            var userRequesting = await _userRepository.Get(userRequestingId);
            foreach (var player in match.PlayerStates.ToList())
            {
                if(player.UserId == userRequestingId)
                {
                    return BadRequest();
                }
            }
            PlayerState playerState = new()
            {
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
