using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Ticket;
using Domain.Entities;

namespace WibuBlog.Controllers
{
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
                return View("AddTicket", addTicketVM); // Chỉ định rõ tên view là "AddTicket"
            }
            try
            {
                await _ticketService.AddNewTicketAsync(addTicketVM);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View("AddTicket", addTicketVM); // Chỉ định rõ tên view là "AddTicket"  
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket is null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            try
            {
                var data = await _ticketService.UpdateTicketAsync(id, ticket);
                return RedirectToAction(nameof(Update), new { id });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View(ticket);
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