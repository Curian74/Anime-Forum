using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> UserProfile(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
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
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            try
            {
                await _userService.UpdateUserAsync(userId, user); 
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

      
    }
}
