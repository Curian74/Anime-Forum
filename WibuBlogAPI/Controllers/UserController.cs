using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Infrastructure.External;
using Application.Interfaces.Email;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
   // [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserService userService, TicketService ticketService) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly TicketService _ticketService = ticketService;

        [HttpGet]
        public async Task<IActionResult> GetAccountDetails()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _userService.GetProfileDetails(userId);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tickets = await _ticketService.GetUserTickets(userId);
            return new JsonResult(Ok(tickets));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            dto.UserId = userId;

            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.Email != dto.Email)
            {
                return new JsonResult(BadRequest("Email không hợp lệ"));
            }

            var validTags = new HashSet<string> { "#BannedAccount", "#HelpCreatePost", "#TechnicalIssue" };
            if (!validTags.Contains(dto.Tag))
            {
                return new JsonResult(BadRequest("Tag không hợp lệ"));
            }

            var result = await _ticketService.CreateTicket(dto);
            return new JsonResult(Ok(result));
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
                return new JsonResult(NotFound("Ticket not found or unauthorized"));
            }
            return new JsonResult(Ok("Ticket deleted successfully"));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
                var result = await _userService.GetProfileDetails(userId);
                if(result == null)
                {
                     return new JsonResult(NotFound());
                }
                return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var result = await _userService.GetUserByEmail(email);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            var result = await _userService.GetUserByUsername(username);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedUsersAsync(int page, int size)
        {
            var result = await _userService.GetPagedUsersAsync(page, size);
            return new JsonResult(Ok(result));
        }
    }
}
