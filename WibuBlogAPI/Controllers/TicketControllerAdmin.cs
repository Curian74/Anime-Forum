using Microsoft.AspNetCore.Mvc;

namespace WibuBlogAPI.Controllers
{
    public class TicketControllerAdmin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
