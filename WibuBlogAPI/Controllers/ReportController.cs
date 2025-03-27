using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;
using Infrastructure.Extensions;
using System.Linq.Expressions;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController(ReportService reportService, UserService userService, PostService postService) : ControllerBase
    {
        private readonly ReportService _reportService = reportService;
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto addReportDTO)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            addReportDTO.UserId = userId;
            if (!ModelState.IsValid || addReportDTO.PostId == Guid.Empty)
            {
                return BadRequest(new { success = false });
            }
            try
            {
                await _reportService.CreateReportAsync(addReportDTO);
                return Ok(new { success = true, message = Application.Common.MessageOperations.MessageConstants.ME020 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false });
            }
        }
        [HttpPut("reports/{reportId}")]
        public async Task<IActionResult> ApproveReport(Guid reportId, [FromBody] ApproveReportDto dto)
        {
            var result = await _reportService.ApproveReportAsync(reportId, dto.Approval, dto.Note);
            try
            {
                return Ok(new { success = true, message = Application.Common.MessageOperations.MessageConstants.ME020 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ModeratorPolicy")]
        [HttpGet("WithDetails")]
        public async Task<IActionResult> GetAllWithDetails(
    string? filterBy = null,
    string? searchTerm = null,
    string? orderBy = null,
    bool descending = false,
    Guid? PostCategoryId = null) // Thêm tham số PostCategoryId
        {
            Expression<Func<Report, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Report>(filterBy, searchTerm);

            // Nếu có PostCategoryId, thêm điều kiện lọc
            if (PostCategoryId.HasValue)
            {
                Expression<Func<Report, bool>> categoryFilter = r => r.Post.PostCategoryId == PostCategoryId.Value;

                if (filter == null)
                {
                    filter = categoryFilter;
                }
                else
                {
                    var parameter = filter.Parameters[0]; // Lấy parameter từ filter hiện có
                    var body = Expression.AndAlso(filter.Body, Expression.Invoke(categoryFilter, parameter));
                    filter = Expression.Lambda<Func<Report, bool>>(body, parameter);
                }
            }


            Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Report>(orderBy, descending);
            var result = await _reportService.GetReportsWithDetailsAsync(filter, orderExpression);
            return Ok(result);
        }

        [HttpGet("PagedWithDetails")]
        public async Task<IActionResult> GetPagedWithDetails(
            int page = 1,
            int size = 10,
            string? filterBy = null,
            string? searchTerm = null,
            string? orderBy = null,
            bool descending = false,
            Guid? PostCategoryId = null) // Thêm tham số PostCategoryId
        {
            Expression<Func<Report, bool>>? filter = ExpressionBuilder.BuildFilterExpression<Report>(filterBy, searchTerm);

            // Nếu có PostCategoryId, thêm điều kiện lọc
            if (PostCategoryId.HasValue)
            {
                Expression<Func<Report, bool>> categoryFilter = r => r.Post.PostCategoryId == PostCategoryId.Value;

                if (filter == null)
                {
                    filter = categoryFilter;
                }
                else
                {
                    var parameter = filter.Parameters[0]; // Lấy parameter từ filter hiện có
                    var body = Expression.AndAlso(filter.Body, Expression.Invoke(categoryFilter, parameter));
                    filter = Expression.Lambda<Func<Report, bool>>(body, parameter);
                }
            }


            Func<IQueryable<Report>, IOrderedQueryable<Report>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Report>(orderBy, descending);
            var result = await _reportService.GetPagedReportsWithDetailsAsync(page, size, filter, orderExpression);
            return Ok(result);
        }

    }
}