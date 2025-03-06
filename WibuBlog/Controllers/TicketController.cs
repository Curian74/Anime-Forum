using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class TicketController(TicketService ticketService) : Controller
    {
        private readonly TicketService _ticketService = ticketService;

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
        public async Task<IActionResult> Detail(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var model = new TicketDetailVM
            {
                Id = ticket.Id,
                Email = ticket.Email,
                Content = ticket.Content,
                Tag = ticket.Tag,
                IsApproved = ticket.IsApproved ?? false
            };

            return View(model);
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
                var success = await _ticketService.UpdateTicketAsync(id, model);

                if (success)
                {
                    TempData["SuccessMessage"] = "Ticket updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update ticket.";
                }

                return RedirectToAction("Detail", new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the ticket.");
                return View("Detail", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}