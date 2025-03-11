using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WibuBlog.ViewModels.Users;
using Application.Common.MessageOperations;
using Microsoft.AspNetCore.Identity;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class UserController(UserService userService) : Controller
    {
        private readonly UserService _userService = userService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var result = await _userService.GetUserProfile();
            if (result is null)
            {
                return NotFound();
            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> UserList(int page = 1, int pageSize = 10)
        {

            var result = await _userService.GetPagedUsersAsync(page, pageSize);
            return View(result);
        }

        [HttpPost]
		public async Task<IActionResult> UpdateUser(UpdateUserVM updateUserVM)
        {
			if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UserProfile));
            }
			
			var user = await _userService.UpdateUserAsync(updateUserVM);
            return RedirectToAction(nameof(UserProfile));
        }

		[HttpPost]
		public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordVM model)
		{
            Console.WriteLine("=============================================");
            Console.WriteLine(model.OldPassword);
            Console.WriteLine(model.ConfirmPassword);
            Console.WriteLine(model.NewPassword);
            Console.WriteLine(model.UserId);
            if (model.NewPassword != model.ConfirmPassword)
			{
				return BadRequest(MessageConstants.ME006);
			}
            var response = await _userService.UpdatePassword(model);
            if (response.Succeeded)
            {
                return Ok(MessageConstants.ME007a);
            }
            return BadRequest(response.Errors[0].Description);
        }
	}
}
