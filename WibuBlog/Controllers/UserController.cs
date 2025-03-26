using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.ViewModels.Users;
using Application.Common.MessageOperations;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class UserController(UserService userService, IWebHostEnvironment webHostEnvironment) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

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

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePhoto(IFormFile file)
        {
			var response = await _userService.UpdateProfilePhoto(file);
            return RedirectToAction(nameof(UserProfile));
                        
        }
    }
}
