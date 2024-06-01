using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MafiaAPI.Data;
using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MafiaAPI.RequestModels;
using Newtonsoft.Json;
using System.Security.Claims;
namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login", Name = "LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] UserAuthRequest userAuthRequest)
        {
            if (userAuthRequest == null)
            {
                return BadRequest();
            }
            User user = await _userRepository.GetByName(userAuthRequest.Name);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Password != SHA256Helper.ComputeSHA256(userAuthRequest.Password))
            {
                return Unauthorized();
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Id) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }

        [Authorize]
        [HttpPost("logout", Name = "LogoutUser")]
        public async Task<IActionResult> LogoutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("register", Name = "RegisterUser")]
        public IActionResult RegisterUser([FromBody] UserAuthRequest userAuthRequest)
        {
            if (userAuthRequest == null)
            {
                return BadRequest();
            }
            if (_userRepository.GetByName(userAuthRequest.Name) != null)
            {
                return Conflict();
            }
            User user = new()
            {
                Name = userAuthRequest.Name,
                Password = SHA256Helper.ComputeSHA256(userAuthRequest.Password)
            };
            _userRepository.Create(user);
            return Ok();
        }

        [Authorize]
        [HttpGet(Name = "GetMatch")]
        public async Task<IActionResult> GetCurrentMatch()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userRepository.Get(userId);

            foreach (var ps in user.PlayerStates)
            {
                if (ps.Match.MatchEnd == null)
                {
                    return Ok(ps.Match.Id);
                }
            }
            return NotFound();
        }
    }
}
