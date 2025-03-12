﻿using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;


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
        public async Task<IActionResult> GetPagedUsers(int page, int size)
        {
            var result = await _userService.GetPagedUsersAsync(page, size);
            return new JsonResult(Ok(result));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserDto updateUserDTO)
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
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordDTO updatePasswordDTO)
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

	}
}
