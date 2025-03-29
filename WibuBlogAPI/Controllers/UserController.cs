using Microsoft.AspNetCore.Mvc;
using Application.Services;
using System.Security.Claims;
using Domain.Entities;
using Application.DTO;
using Microsoft.AspNetCore.SignalR;
using Application.Common.MessageOperations;
using Infrastructure.Extensions;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlogAPI.Controllers
{
    // [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        private readonly UserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetAccountDetails(Guid? userId)
        {
            if (userId == null)
            {
                userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

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
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmail(email);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var result = await _userService.GetUserByUsername(username);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int page = 1,
            int size = 10,
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<User, bool>>? filter = ExpressionBuilder.BuildFilterExpression<User>(filterBy, searchTerm);
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderExpression = ExpressionBuilder.BuildOrderExpression<User>(orderBy, descending);

            var result = await _userService.GetPagedAsync(page, size, filter, orderExpression);

            return new JsonResult(Ok(result));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDTO)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == updateUserDTO.Id.ToString()) await _userService.UpdateUserAsync(updateUserDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(updateUserDTO));
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO updatePasswordDTO)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == updatePasswordDTO.UserId.ToString())
                    return new JsonResult(await _userService.UpdatePasswordAsync(updatePasswordDTO));
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }
            return new JsonResult(BadRequest());
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfilePhoto([FromBody] Media media)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return new JsonResult(await _userService.UpdateProfilePhotoAsync(media, currentUserId));

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"file/server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _userService.GetUserNotification(currentUserId);
                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"file/server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false)
        {
            Expression<Func<User, bool>>? filter = ExpressionBuilder.BuildFilterExpression<User>(filterBy, searchTerm);
            Func<IQueryable<User>, IOrderedQueryable<User>>? orderExpression = ExpressionBuilder.BuildOrderExpression<User>(orderBy, descending);

            var result = await _userService.GetAllAsync(filter, orderExpression);

            return new JsonResult(Ok(result.Items));
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> ToggleBan(Guid userId)
        {
            try
            {
                await _userService.ToggleBanAsync(userId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());

        }
    }
}
