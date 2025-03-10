using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Infrastructure.External;
using Application.Interfaces.Email;
using Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;


        [HttpGet]
        public async Task<IActionResult> GetAccountDetails()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _userService.GetProfileDetails(userId);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(result));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(user));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var result = await _userService.GetUserByEmail(email);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            var result = await _userService.GetUserByUsername(username);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedUsersAsync(int page, int size)
        {
            var result = await _userService.GetPagedUsersAsync(page, size);
            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserAsync([FromBody]UpdateUserDto updateUserDTO)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == updateUserDTO.Id.ToString()) await _userService.UpdateUser(updateUserDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(updateUserDTO));
        }
    }
}
