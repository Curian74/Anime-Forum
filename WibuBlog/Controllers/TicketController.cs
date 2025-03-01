using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class TicketController(TicketServices ticketService) : Controller
    {
        private readonly TicketServices _ticketService = ticketService;

        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _ticketService.GetPagedTicketAsync(page, pageSize);
            return View("Index", value);
        }

        public async Task<IActionResult> NewTickets(int? page = 1, int? pageSize = 10)
        {
            var value = await _ticketService.GetPagedTicketAsync(page, pageSize);
            return View(value);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("AddTicket");
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTicketVM addTicketVM)
        {
            if (!ModelState.IsValid)
            {
                return View("AddTicket", addTicketVM);
            }

            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in.";
                    return RedirectToAction(nameof(Add));
                }

                addTicketVM.UserId = Guid.Parse(userId); 

                await _ticketService.AddNewTicketAsync(addTicketVM);
                TempData["SuccessMessage"] = "Ticket submitted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting the ticket.";
            }

            return RedirectToAction(nameof(Add));
        }



        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket is null)
            {
                return NotFound();
            }

            var model = new TicketDetailVM
            {
                Id = ticket.Id,
                Email = ticket.Email,
                Content = ticket.Content,
                Tag = ticket.Tag,
                IsApproved = ticket.IsApproved
            };

            return View("Detail", model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, TicketDetailVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Detail", model);
            }

            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket is null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin ticket
                ticket.Email = model.Email;
                ticket.Content = model.Content;
                ticket.Tag = model.Tag;
                ticket.IsApproved = model.IsApproved;

                await _ticketService.UpdateTicketAsync(id, ticket);

                TempData["SuccessMessage"] = "Ticket updated successfully!";
                return RedirectToAction(nameof(Update), new { id });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the ticket.");
                return View("Detail", model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || ticket.IsApproved != null)
            {
                TempData["ErrorMessage"] = "Ticket cannot be canceled.";
                return RedirectToAction(nameof(Update), new { id });
            }

            ticket.IsApproved = false; 
            await _ticketService.UpdateTicketAsync(id, ticket);

            TempData["SuccessMessage"] = "Ticket has been canceled.";
            return RedirectToAction(nameof(Update), new { id });
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}