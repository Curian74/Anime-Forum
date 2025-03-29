using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static Domain.ValueObjects.Enums.TicketStatusEnum;
using Domain.Entities;
using Infrastructure.Extensions;
using System.Linq.Expressions;

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

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpGet()]
        public async Task<IActionResult> GetUserTickets()
        {
            var userId = GetUserId();
            if (userId == null) return new JsonResult(Unauthorized("Invalid user ID"));

            var (cacacaaca, count) = await _ticketService.GetUserTicketsAsync(userId.Value);
            return new JsonResult(Ok(cacacaaca));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            dto.UserId = userId;
            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.Email != dto.Email) return new JsonResult(BadRequest("Email không hợp lệ"));

            var validTags = new HashSet<string> { "#BannedAccount", "#HelpCreatePost", "#TechnicalIssue" };
            if (!validTags.Contains(dto.Tag)) return new JsonResult(BadRequest("Tag không hợp lệ"));

            var result = await _ticketService.CreateTicketAsync(dto);
            return new JsonResult(Ok(new { success = result > 0 }));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketDetails(Guid ticketId)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null)
                return new JsonResult(NotFound("Ticket not found"));

            var userId = GetUserId();
            if (ticket.Status == TicketStatus.Closed &&
                userId != ticket.UserId &&
                !User.IsInRole("Admin")) 
            {
                return new JsonResult(Forbid("Closed tickets can only be viewed by their creators"));
            }

            return new JsonResult(Ok(ticket));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket(Guid ticketId, UpdateTicketDto dto)
        {
            var result = await _ticketService.UpdateTicketAsync(ticketId, dto);
            if (result == 0) return new JsonResult(NotFound("Ticket not found or unauthorized"));
            return new JsonResult(Ok("Ticket updated successfully"));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(Guid ticketId)
        {
            var result = await _ticketService.DeleteTicketAsync(ticketId);
            if (result == 0) return new JsonResult(NotFound("Ticket not found"));
            return new JsonResult(Ok("Ticket deleted successfully"));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var (allTickets, count) = await _ticketService.GetAllTicketsAsync();

            var tickets = allTickets.Where(t => t.Status != TicketStatus.Closed).ToList();

            return new JsonResult(Ok(new { tickets, count = tickets.Count }));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetPagedTickets(
            int page = 1,
            int size = 10,
            string? orderBy = "CreatedAt",
            bool descending = false)
        {

            //if (PostCategoryId.HasValue)
            //{
            //    //Expression<Func<Ticket, bool>> categoryFilter = r => r.Post.PostCategoryId == PostCategoryId.Value;

            //    if (filter == null)
            //    {
            //        filter = categoryFilter;
            //    }
            //    else
            //    {
            //        var parameter = filter.Parameters[0];
            //        var body = Expression.AndAlso(filter.Body, Expression.Invoke(categoryFilter, parameter));
            //        filter = Expression.Lambda<Func<Ticket, bool>>(body, parameter);
            //    }
            //}

            Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>? orderExpression = ExpressionBuilder.BuildOrderExpression<Ticket>(orderBy, descending);
            var result = await _ticketService.GetPagedAsync(page, size);
            return new JsonResult(Ok(result));
        }

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        [HttpPost("{ticketId}")]
        public async Task<IActionResult> ApproveTicket(Guid ticketId, [FromBody] ApproveTicketDto dto)
        {
            var result = await _ticketService.ApproveTicketAsync(ticketId, dto.Approval, dto.Note);
            if (result == 0) return new JsonResult(NotFound("Ticket not found"));
            return new JsonResult(Ok("Ticket approved"));
        }

        [HttpPut("{ticketId}")]
        public async Task<IActionResult> CloseTicket(Guid ticketId)
        {
            var userId = GetUserId();
            if (userId == null) return new JsonResult(Unauthorized("Invalid user ID"));

            var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
            if (ticket == null) return new JsonResult(NotFound("Ticket not found"));

            if (ticket.UserId != userId.Value)
                return new JsonResult(Forbid("Only the ticket creator can close this ticket"));

            var result = await _ticketService.CloseTicketAsync(ticketId);
            if (result == 0) return new JsonResult(NotFound("Ticket not found"));

            return new JsonResult(Ok("Ticket closed successfully"));
        }
    }
}
