using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController(AdminService adminServices) : ControllerBase
    {
        private readonly AdminService _adminService = adminServices;
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUserList = await _adminService.GetAllUsersAsync();
            return new JsonResult(Ok(allUserList.Items));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _adminService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(user));
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _adminService.UpdateUserAsync(userId, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(dto));
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId, [FromBody] UserProfileDto dto)
        {
            try
            {
                await _adminService.DeleteUserAsync(userId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());
        }

        [HttpGet]
        public async Task<IActionResult> GetStats(int days = 7)
        {
            var webStats = await _adminService.GetStats(days);
            return new JsonResult(Ok(webStats));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleModeratorRole(Guid userId)
        {
            int? result;
            try { result = await _adminService.ToggleModeratorRoleAsync(userId); }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Ok(result));
        }
    }
}
