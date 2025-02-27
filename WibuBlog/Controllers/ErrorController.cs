using Microsoft.AspNetCore.Mvc;

namespace WibuBlog.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/NotFound")]
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
