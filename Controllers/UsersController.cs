using Microsoft.AspNetCore.Mvc;
using MafiaAPI.Models;
using MafiaAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MafiaAPI.Schemas;
using MafiaAPI.RequestModels;
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
            await Console.Out.WriteLineAsync($"Request: {userAuthRequest.login}");
            if (userAuthRequest == null)
            {
                return BadRequest();
            }
            User user = await _userRepository.GetByName(userAuthRequest.login);
            if (user == null)
            {
                return NotFound();
            }
            if (user.Password != SHA256Helper.ComputeSHA256(userAuthRequest.password))
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
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest userRegisterRequest)
        {
            if (userRegisterRequest == null)
            {
                return BadRequest();
            }
            if(userRegisterRequest.password1 != userRegisterRequest.password2)
            {
                return BadRequest();
            }
            if (await _userRepository.GetByName(userRegisterRequest.login) != null)
            {
                return Conflict();
            }
            User user = new()
            {
                Name = userRegisterRequest.login,
                Password = SHA256Helper.ComputeSHA256(userRegisterRequest.password1)
            };
            await _userRepository.Create(user);
            return Ok();
        }

        [HttpGet(Name = "GetUser")]
        public async Task<IActionResult> GetUser()
        {
            if(User.Identity.IsAuthenticated)
            {
                string? userId = User.Identity.Name;
                User user = await _userRepository.Get(userId);

                foreach (var ps in user.PlayerStates)
                {
                    if (ps.Match.MatchEnd == null)
                    {
                        return Ok(new AuthenticatedUser()
                        {
                            id = user.Id,
                            name = user.Name,
                            matchInProgress = true,
                            matchInProgressId = ps.MatchId
                        });
                    }
                }
                return Ok(new AuthenticatedUser()
                {
                    id = user.Id,
                    name = user.Name,
                    matchInProgress = false
                });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
