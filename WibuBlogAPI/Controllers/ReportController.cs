using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController(ReportService reportService, UserService userService, PostService postService) : ControllerBase
    {
        private readonly ReportService _reportService = reportService;
        private readonly UserService _userService = userService;
        private readonly PostService _postService = postService;

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto addReportDTO)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            addReportDTO.UserId = userId;
            if (!ModelState.IsValid || addReportDTO.PostId == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _reportService.CreateReportAsync(addReportDTO);
                return Ok(new { message = "Report created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to create report. Error: {ex.Message}" });
            }
        }
    }
}