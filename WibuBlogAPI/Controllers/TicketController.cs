using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController(UserService userService, TicketService ticketService) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly TicketService _ticketService = ticketService;

        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet("UserTickets")]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid user ID");

            var (tickets, count) = await _ticketService.GetUserTicketsAsync(userId.Value);
            return Ok(new { tickets, count });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized("Invalid user ID");

            dto.UserId = userId.Value;
            var user = await _userService.FindByIdAsync(userId.Value);
            if (user == null || user.Email != dto.Email) return BadRequest("Email không hợp lệ");

            var validTags = new HashSet<string> { "#BannedAccount", "#HelpCreatePost", "#TechnicalIssue" };
            if (!validTags.Contains(dto.Tag)) return BadRequest("Tag không hợp lệ");

            var result = await _ticketService.CreateTicketAsync(dto);
            return Ok(new { success = result > 0 });
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketDetail(Guid ticketId)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null)
                return NotFound("Ticket not found");

            return new JsonResult(Ok(ticket));
        }

        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket(Guid ticketId, UpdateTicketDto dto)
        {
            var result = await _ticketService.UpdateTicketAsync(ticketId, dto);
            if (result == 0) return NotFound("Ticket not found or unauthorized");
            return Ok("Ticket updated successfully");
        }

        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(Guid ticketId)
        {
            var result = await _ticketService.DeleteTicketAsync(ticketId);
            if (result == 0) return NotFound("Ticket not found");
            return Ok("Ticket deleted successfully");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpGet("AllTickets")]
        public async Task<IActionResult> GetAllTickets()
        {
            var (tickets, count) = await _ticketService.GetAllTicketsAsync();
            return Ok(new { tickets, count });
        }



        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpPut("Approve/{ticketId}")]
        public async Task<IActionResult> ApproveTicket(Guid ticketId, [FromBody] ApproveTicketDto dto)
        {
            var result = await _ticketService.ApproveTicketAsync(ticketId, dto.Approval, dto.Note);
            if (result == 0) return NotFound("Ticket not found");
            return Ok("Ticket approved");
        }
    }

    public record ApproveTicketDto(bool Approval, string? Note);
}
