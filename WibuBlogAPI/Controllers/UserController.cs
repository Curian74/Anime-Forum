using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Infrastructure.Configurations;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
   // [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(UserServices userServices, TicketServices ticketServices, JwtHelper jwtHelper,
        IOptions<AuthTokenOptions> authTokenOptions) : ControllerBase
    {
        private readonly UserServices _userServices = userServices;
        private readonly TicketServices _ticketServices = ticketServices;
        private readonly JwtHelper _jwtHelper = jwtHelper;
       
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;

        [HttpGet]
        public async Task<IActionResult> GetAccountDetails()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _userServices.GetProfileDetails(userId);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var tickets = await _ticketServices.GetUserTickets(userId);
            return new JsonResult(Ok(tickets));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            dto.UserId = userId;

            // Kiểm tra xem email có thuộc user không
            var user = await _userServices.FindByIdAsync(userId);
            if (user == null || user.Email != dto.Email)
            {
                return new JsonResult(BadRequest("Email không hợp lệ"));
            }

            // Danh sách tag hợp lệ
            var validTags = new HashSet<string> { "#BannedAccount", "#HelpCreatePost", "#TechnicalIssue" };
            if (!validTags.Contains(dto.Tag))
            {
                return new JsonResult(BadRequest("Tag không hợp lệ"));
            }

            var result = await _ticketServices.CreateTicket(dto);
            return new JsonResult(Ok(result));
        }


        [HttpPut]
        public async Task<IActionResult> UpdateTicket(UpdateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _ticketServices.UpdateTicket(dto, userId);
            if (!result)
            {
                return new JsonResult(NotFound("Ticket not found or unauthorized"));
            }
            return new JsonResult(Ok("Ticket updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _ticketServices.DeleteTicket(id, userId);
            if (!result)
            {
                return new JsonResult(NotFound("Ticket not found or unauthorized"));
            }
            return new JsonResult(Ok("Ticket deleted successfully"));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
                var result = await _userServices.GetProfileDetails(userId);
                if(result == null)
                {
                     return new JsonResult(NotFound());
                }
                return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var result = await _userServices.GetUserByEmail(email);
            if (result == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedUsersAsync(int page, int size)
        {
            var result = await _userServices.GetPagedUsersAsync(page, size);
            return new JsonResult(Ok(result));
        }

      
    }
}
