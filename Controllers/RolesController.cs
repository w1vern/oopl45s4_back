using MafiaAPI.Repositories;
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


    }
}
