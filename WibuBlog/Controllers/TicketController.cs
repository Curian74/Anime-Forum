using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class TicketController(TicketService ticketService) : Controller
    {
        private readonly TicketService _ticketService = ticketService;

        public async Task<IActionResult> Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> NewTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var tickets = await _ticketService.GetUserTicketsAsync(userId);
            return View(tickets);
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
                Status = ticket.Status,
                Note = ticket.Note,
                CreatedAt = ticket.CreatedAt,
                LastModifiedAt = ticket.LastModifiedAt,
                UserId = ticket.UserId,
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
        [HttpPost]
        public async Task<IActionResult> Close(Guid id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not logged in.";
                    return RedirectToAction(nameof(Index));
                }

                var ticket = await _ticketService.GetTicketByIdAsync(id);

                if (ticket.UserId.ToString() != userId)
                {
                    TempData["ErrorMessage"] = "You can only close tickets you created.";
                    return RedirectToAction(nameof(Detail), new { id });
                }

                var success = await _ticketService.CloseTicketAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Ticket closed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to close ticket.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while closing the ticket.";
                return RedirectToAction(nameof(Detail), new { id });
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