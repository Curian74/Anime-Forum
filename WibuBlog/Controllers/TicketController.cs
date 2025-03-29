using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Common.Pagination;
using WibuBlog.Helpers;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class TicketController(TicketService ticketService) : Controller
    {
        private readonly TicketService _ticketService = ticketService;

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var tickets = await _ticketService.GetPagedAsync(page, 10, false);

            var data = new TicketsVM { Tickets = tickets };

            return View(data);
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
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

            var model = new AddTicketVM
            {
                Email = userEmail,
                Tag = string.Empty,
                Content = string.Empty
            };

            return View("AddTicket", model);
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
                    return RedirectToAction(nameof(Add));
                }

                addTicketVM.UserId = Guid.Parse(userId); 

                await _ticketService.AddNewTicketAsync(addTicketVM);
            }
            catch (Exception)
            {
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

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "ModeratorPolicy")]
        public async Task<IActionResult> ViewTickets([FromQuery] QueryObject queryObject)
        {
            var ticketList = await _ticketService.GetAllTicketsAsync();

            if (!string.IsNullOrEmpty(queryObject.SearchTerm))
            {
                ticketList = ticketList
                    .Where(t =>
                        (t.User.UserName ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (t.Content ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (t.Tag ?? "").Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            if (!string.IsNullOrEmpty(queryObject.FilterBy))
            {
                ticketList = queryObject.FilterBy.ToLower() switch
                {
                    "pending" => ticketList.Where(t => t.Status == Domain.ValueObjects.Enums.TicketStatusEnum.TicketStatus.Pending).ToList(),
                    "approved" => ticketList.Where(t => t.Status == Domain.ValueObjects.Enums.TicketStatusEnum.TicketStatus.Approved).ToList(),
                    "rejected" => ticketList.Where(t => t.Status == Domain.ValueObjects.Enums.TicketStatusEnum.TicketStatus.Rejected).ToList(),
                    _ => ticketList
                };
            }

            if (!string.IsNullOrEmpty(queryObject.TagFilter))
            {
                ticketList = ticketList.Where(t => t.Tag == queryObject.TagFilter).ToList();
            }

            ticketList = (queryObject.OrderBy?.ToLower(), queryObject.Descending) switch
            {
                ("createdat", true) => ticketList.OrderByDescending(x => x.CreatedAt).ToList(),
                ("createdat", false) => ticketList.OrderBy(x => x.CreatedAt).ToList(),
                _ => ticketList
            };

            int totalItems = ticketList.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryObject.Size);
            int skip = (queryObject.Page - 1) * queryObject.Size;
            var pagedTickets = ticketList.Skip(skip).Take(queryObject.Size);

            var data = new TicketsVM
            {
                Tickets = new PagedResult<Ticket>(pagedTickets, totalItems, queryObject.Page, queryObject.Size),
            };

            return View("Index", data);
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

        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        public async Task<IActionResult> ApproveReport(Guid ticketId, bool approval, string? note = null)
        {
            try
            {
                var result = await _ticketService.ApproveTicketAsync(ticketId, approval, note);

                return Json(new { success = true, message = Application.Common.MessageOperations.MessageConstants.ME020 });

            }
            catch (Exception ex)
            {
                return Json(new { success = false });

            }
        }
    }
}