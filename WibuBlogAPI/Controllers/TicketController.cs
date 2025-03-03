using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Infrastructure.External;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlogAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketController(UserService userService, TicketService ticketService, EmailService emailService) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly TicketService _ticketService = ticketService;
        private readonly EmailService _emailService = emailService;

        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tickets = await _ticketService.GetUserTickets(userId);
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            dto.UserId = userId;

            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.Email != dto.Email)
            {
                return BadRequest("Email không hợp lệ");
            }

            var validTags = new HashSet<string> { "#BannedAccount", "#HelpCreatePost", "#TechnicalIssue" };
            if (!validTags.Contains(dto.Tag))
            {
                return BadRequest("Tag không hợp lệ");
            }

            var result = await _ticketService.CreateTicket(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTicket(UpdateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _ticketService.UpdateTicket(dto, userId);

            if (!result)
            {
                return NotFound(new { success = false, message = "Ticket not found or unauthorized" });
            }

            return Ok(new { success = true, message = "Ticket updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _ticketService.DeleteTicket(id, userId);
            if (!result)
            {
                return NotFound("Ticket not found or unauthorized");
            }
            return Ok("Ticket deleted successfully");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketDetail(Guid ticketId)
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var ticketDetail = tickets.Find(t => t.Id == ticketId);
            if (ticketDetail == null)
            {
                return NotFound("Ticket not found");
            }
            return Ok(ticketDetail);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpPut("Approve/{ticketId}")]
        public async Task<IActionResult> ApproveTicket(Guid ticketId, [FromBody] string? note)
        {
            var result = await _ticketService.ApproveTicket(ticketId, note);
            if (!result)
            {
                return NotFound("Ticket not found");
            }
            return Ok("Ticket approved");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpPut("Reject/{ticketId}")]
        public async Task<IActionResult> RejectTicket(Guid ticketId, [FromBody] string? note)
        {
            var result = await _ticketService.RejectTicket(ticketId, note);
            if (!result)
            {
                return NotFound("Ticket not found");
            }
            return Ok("Ticket rejected");
        }
    }
}
