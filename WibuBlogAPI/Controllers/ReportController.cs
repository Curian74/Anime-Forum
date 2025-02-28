using Microsoft.AspNetCore.Mvc;

namespace WibuBlogAPI.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
