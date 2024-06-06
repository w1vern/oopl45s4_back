using MafiaAPI.Repositories;
using MafiaAPI.Schemas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MafiaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        public RolesController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet("/get_roles")]
        public ActionResult GetAllRoles()
        {
            List<RoleResponse> roles = [];
            foreach (var role in _roleRepository.Get().ToList())
            {
                roles.Add(new()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Priority = role.Priority
                });
            }
            return Ok(roles);
        }
    }
}
