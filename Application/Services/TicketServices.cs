using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class TicketServices
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;

        public TicketServices(DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> GetUserTickets(Guid userId)
        {
            var tickets = await _context.Set<Ticket>()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDto?> CreateTicket(CreateTicketDto dto)
        {
            var ticket = _mapper.Map<Ticket>(dto);
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.LastModifiedAt = DateTime.UtcNow;

            await _context.Set<Ticket>().AddAsync(ticket);
            await _context.SaveChangesAsync();

            return _mapper.Map<TicketDto>(ticket);
        }

        public async Task<bool> UpdateTicket(UpdateTicketDto dto, Guid userId)
        {
            var ticket = await _context.Set<Ticket>()
                .FirstOrDefaultAsync(t => t.Id == dto.Id && t.UserId == userId);

            if (ticket == null)
            {
                return false;
            }

            _mapper.Map(dto, ticket);
            ticket.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTicket(Guid id, Guid userId)
        {
            var ticket = await _context.Set<Ticket>()
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
            {
                return false;
            }

            _context.Set<Ticket>().Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveTicket(Guid id, string? note = null)
        {
            var ticket = await _context.Set<Ticket>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return false;
            }

            ticket.IsApproved = true;
            ticket.Note = note;
            ticket.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTicket(Guid id, string? note = null)
        {
            var ticket = await _context.Set<Ticket>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return false;
            }

            ticket.IsApproved = false;
            ticket.Note = note;
            ticket.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}