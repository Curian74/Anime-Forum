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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.Controllers
{
    public class ReportController(ReportService reportService, UserService userService, PostService postService, PostCategoryService categoryService) : Controller
    {
        private readonly ReportService _reportService = reportService;
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;
        private readonly PostCategoryService _postcategoryService = categoryService;


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
            // Fetch all categories
            var categories = await _postcategoryService.GetAllCategories(null, null, false);

            var reportList = await _reportService.GetAllReportWithDetailsAsync(null, null, false);

            // Category filtering with correct null handling
            if (queryObject.PostCategoryId != Guid.Empty)
            {
                reportList = reportList
                    .Where(x => x.PostCategoryId == queryObject.PostCategoryId)
                    .ToList();
            }

            // Existing search term filtering
            if (!string.IsNullOrEmpty(queryObject.SearchTerm))
            {
                reportList = reportList
                    .Where(x =>
                        (x.Reason ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (x.Username ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (x.PostTitle ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // Existing status filtering
            if (!string.IsNullOrEmpty(queryObject.FilterBy))
            {
                reportList = queryObject.FilterBy.ToLower() switch
                {
                    "pending" => reportList.Where(x => !x.IsApproved.HasValue).ToList(),
                    "approved" => reportList.Where(x => x.IsApproved == true).ToList(),
                    "rejected" => reportList.Where(x => x.IsApproved == false).ToList(),
                    _ => reportList
                };
            }

            // Existing sorting logic
            reportList = (queryObject.OrderBy?.ToLower(), queryObject.Descending) switch
            {
                ("createdat", true) => reportList.OrderByDescending(x => x.CreatedAt).ToList(),
                ("createdat", false) => reportList.OrderBy(x => x.CreatedAt).ToList(),
                _ => reportList
            };

            int totalItems = reportList.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryObject.Size);
            int skip = (queryObject.Page - 1) * queryObject.Size;
            var pagedReports = reportList.Skip(skip).Take(queryObject.Size).ToList();

            var data = new ReportsVM
            {
                Reports = new PagedResult<ReportDto>(pagedReports, totalItems, queryObject.Page, queryObject.Size),
                CategoryList = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == queryObject.PostCategoryId
                }).ToList(),
                PostCategoryId = queryObject.PostCategoryId
            };

            return View("Reports", data);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ModeratorPolicy")]
        public async Task<IActionResult> ApproveReport(Guid reportId, bool approval, string? note = null)
        {
            try
            {
                var result = await _reportService.ApproveReportAsync(reportId, approval, note);

                return Json(new { success = true, message = Application.Common.MessageOperations.MessageConstants.ME020 });

            }
            catch (Exception ex)
            {
                return Json(new { success = false });

            }
        }
    }
}