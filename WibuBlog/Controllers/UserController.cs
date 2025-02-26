using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;

namespace WibuBlog.Controllers
{
  
    public class UserController(UserServices userServices) : Controller
    {
        private readonly UserServices _userServices = userServices;
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid id)
        {
            var result = await _userServices.GetUserByIdAsync(id);
            if (result is null)
            {
                return NotFound();
            }
            
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> UserList(int page = 1, int pageSize = 10)
        {
            var result = await _userServices.GetPagedUsersAsync(page, pageSize);
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
                await _userServices.UpdateUserAsync(userId, user); 
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

    }
}
