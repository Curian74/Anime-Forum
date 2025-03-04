using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class TicketService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Ticket> _ticketRepository = unitOfWork.GetRepository<Ticket>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<Ticket>, int)> GetUserTickets(Guid userId)
        {
            var result = await _ticketRepository.GetAllAsync(t => t.UserId.Equals(userId), t => t.OrderByDescending(t => t.CreatedAt));

            return result;
        }

        public async Task<Ticket?> GetTicketById(Guid ticketId)
        {
            return await _ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<int> CreateTicket(CreateTicketDto dto)
        {
            var ticket = _mapper.Map<Ticket>(dto);

            await _ticketRepository.AddAsync(ticket);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateTicket(Guid ticketId, UpdateTicketDto dto)
        {
            var ticket = _mapper.Map<Ticket>(dto);

            ticket.Id = ticketId;

            await _ticketRepository.UpdateAsync(ticket);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteTicket(Guid ticketId)
        {
            await _ticketRepository.DeleteAsync(ticketId);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> ApproveTicket(Guid ticketId, bool approval, string? note = null)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket != null)
            {
                ticket.IsApproved = approval;
                ticket.Note = note;
                await _ticketRepository.UpdateAsync(ticket);
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Ticket>, int)> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

    }
}