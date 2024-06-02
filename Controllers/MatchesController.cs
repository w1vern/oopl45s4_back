using MafiaAPI.Models;
using MafiaAPI.Repositories;
using MafiaAPI.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;

        public MatchesController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [Authorize]
        [HttpPost("start", Name = "StartMatch")]
        public async Task<IActionResult> StartMatch([FromBody] string id)
        {
            string? userRequestingId = User.Identity.Name;
            Match match = await _matchRepository.Get(id);
            foreach (var ps in match.PlayerStates)
            {
                if (ps.Role == "Host")
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
        [HttpGet(Name = "GetWebsocketURL")]
        public async Task<IActionResult> GetWebsocketURL([FromBody] string id)
        {
            var match = await _matchRepository.Get(id);
            if (match == null)
            {
                return BadRequest();
            }
            if (match.MatchEnd != null)
            {
                return NotFound();
            }
            return Ok(match.WebsocketURL);
        }

        [Authorize]
        [HttpGet("{id:alpha}", Name = "GetMatchInfo")]
        public async Task<IActionResult> GetInfo(string id)
        {
            var match = await _matchRepository.Get(id);
            if (match == null)
            {
                return BadRequest();
            }
            MatchRequest matchRequest = new()
            {
                Id = match.Id,
                MatchStart = match.MatchStart,
                MatchEnd = match.MatchEnd,
                MatchResult = match.MatchResult
            };
            foreach (var ps in match.PlayerStates)
            {
                matchRequest.PlayersIds.Add(ps.User.Id);
            }
            return Ok(matchRequest);
        }

        [Authorize]
        [HttpGet("available", Name = "GetAvailableMatches")]
        public IActionResult GetAvailableMatches()
        {
            List<Match> matchesAvailable = [];
            var matches = _matchRepository.Get();
            foreach (var match in matches)
            {
                if (match.MatchStart == null)
                {
                    matchesAvailable.Add(match);
                }
            }
            return Ok(matchesAvailable);
        }
    }
}
