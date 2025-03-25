using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Report;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.Common.Pagination;
using Application.DTO;

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

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ModeratorPolicy")]
        public async Task<IActionResult> ViewReports([FromQuery] QueryObject queryObject)
        {
            var reportList = await _reportService.GetAllReportWithDetailsAsync(null, null, false);
            if (!string.IsNullOrEmpty(queryObject.SearchTerm))
            {
                reportList = reportList.Where(x => x.Reason.Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            int totalItems = reportList.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryObject.Size);
            int skip = (queryObject.Page - 1) * queryObject.Size;
            var pagedReports = reportList.Skip(skip).Take(queryObject.Size).ToList();
            var data = new ReportsVM
            {
                Reports = new PagedResult<ReportDto>(pagedReports, totalItems, queryObject.Page, queryObject.Size)
            };
            return View("Reports", data);
        }
    }
}