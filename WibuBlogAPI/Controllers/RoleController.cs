using Application.Services;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return new JsonResult(Ok(_roleService.GetAllRolesAsync()));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRole(Guid userId)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return new JsonResult(Ok(await _roleService.GetUserRolesAsync(userId)));
        }
    }
}
