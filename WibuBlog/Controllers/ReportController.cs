using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Report;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlog.Controllers
{
    public class ReportController(ReportService reportService, UserService userService, PostService postService) : Controller
    {
        private readonly ReportService _reportService = reportService;
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;

        [HttpGet]
        public async Task<IActionResult> CreateReport(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var data = new AddReportVM
            {
                UserId = Guid.Parse(userId),
                PostId = postId
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(AddReportVM addReportVM)
        {
            Console.WriteLine($"MVC Controller: PostId={addReportVM.PostId}, Reason={addReportVM.Reason}");
            if (!ModelState.IsValid || addReportVM.PostId == Guid.Empty)
            {
                return Json(new { success = false });
            }
            try
            {
                await _reportService.CreateReportAsync(addReportVM);
                return Json(new { success = true, message = Application.Common.MessageOperations.MessageConstants.ME020 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
}