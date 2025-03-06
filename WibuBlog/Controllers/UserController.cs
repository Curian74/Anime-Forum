using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WibuBlog.ViewModels.Users;

namespace WibuBlog.Controllers
{
   // [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
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
            if (result == null)
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
        public async Task<IActionResult> UpdateUser(EditUserVM editUserVM)
        {
            if (!ModelState.IsValid) {
               RedirectToAction(nameof(UserProfile));
            }
            var user = await _userService.UpdateUserAsync(editUserVM);
        
            return RedirectToAction(nameof(UserProfile));
        }



    }
}
