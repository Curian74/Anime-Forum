using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController (NotificationService notificationService) : Controller
    {
        private readonly NotificationService _notificationService = notificationService;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Notification notification)
        {
            var result = await _notificationService.Add(notification);
            return new JsonResult(Ok(result));
        }
    }
}
