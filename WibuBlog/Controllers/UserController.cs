using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
