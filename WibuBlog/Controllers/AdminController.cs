using Microsoft.AspNetCore.Mvc;
using Application.DTO;
using WibuBlog.Services;

namespace WibuBlog.Controllers
{
    public class AdminController(AdminService adminService) : Controller
    {
        private readonly AdminService _adminService = adminService;

        public async Task<IActionResult> Dashboard()
        {
            var webStats = await _adminService.GetStatsAsync(30);

            return View(webStats);
        }
    }
}
