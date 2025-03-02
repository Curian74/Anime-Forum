﻿using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WibuBlogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController(AdminService adminServices, TicketService ticketServices) : ControllerBase
    {
        private readonly AdminService _adminServices = adminServices;
        private readonly TicketService _ticketServices = ticketServices;

        private readonly AdminService _adminService = adminServices;
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUserList = await _adminService.GetAllUsersAsync();
            return new JsonResult(Ok(allUserList.Items));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _adminService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new JsonResult(NotFound());
            }
            return new JsonResult(Ok(user));
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _adminService.UpdateUserAsync(userId, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(Accepted(dto));
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId, [FromBody] UserProfileDto dto)
        {
            try
            {
                await _adminService.DeleteUserAsync(userId);
            }
            catch (KeyNotFoundException ex)
            {
                return new JsonResult(NotFound($"{ex.GetType().Name}: {ex.Message}"));
            }

            return new JsonResult(NoContent());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketServices.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketDetail(Guid ticketId)
        {
            var ticket = await _ticketServices.GetAllTicketsAsync();
            var ticketDetail = ticket.Find(t => t.Id == ticketId);
            if (ticketDetail == null)
            {
                return NotFound("Ticket not found");
            }
            return Ok(ticketDetail);
        }

        [HttpPut("Approve/{ticketId}")]
        public async Task<IActionResult> ApproveTicket(Guid ticketId, [FromBody] string? note)
        {
            var result = await _ticketServices.ApproveTicket(ticketId, note);
            if (!result)
            {
                return NotFound("Ticket not found");
            }
            return Ok("Ticket approved");
        }

        [HttpPut("Reject/{ticketId}")]
        public async Task<IActionResult> RejectTicket(Guid ticketId, [FromBody] string? note)
        {
            var result = await _ticketServices.RejectTicket(ticketId, note);
            if (!result)
            {
                return NotFound("Ticket not found");
            }
            return Ok("Ticket rejected");
        }
    }
}
