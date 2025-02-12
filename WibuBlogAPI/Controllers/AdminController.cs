using Application.DTO;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController(AdminService adminServices) : ControllerBase
    {
        private readonly AdminService _adminServices = adminServices;
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUserList = await _adminServices.GetAllUsersAsync();
            return new JsonResult(Ok(allUserList));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _adminServices.GetUserByIdAsync(userId);
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
                await _adminServices.UpdateUserAsync(userId, dto);
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
                await _adminServices.DeleteUserAsync(userId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());
        }
    }
}
