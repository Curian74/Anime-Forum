using Application.Common.Pagination;
using Application.DTO;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;
using static Domain.ValueObjects.Enums.TicketStatusEnum;

namespace Application.Services
{
    public class TicketService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Ticket> _ticketRepository = unitOfWork.GetRepository<Ticket>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<Ticket>, int)> GetUserTicketsAsync(Guid userId)
        {
            var result = await _ticketRepository.GetAllAsync(t => t.UserId.Equals(userId), t => t.OrderByDescending(t => t.CreatedAt));

            return result;
        }

        public async Task<Ticket?> GetTicketByIdAsync(Guid ticketId)
        {
            return await _ticketRepository.GetByIdAsync(ticketId);
        }

        public async Task<int> CreateTicketAsync(CreateTicketDto dto)
        {
            var ticket = _mapper.Map<Ticket>(dto);

            await _ticketRepository.AddAsync(ticket);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateTicketAsync(Guid ticketId, UpdateTicketDto dto)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(ticketId);

            if (existingTicket == null)
                return 0;

            if (!string.IsNullOrEmpty(dto.Content))
                existingTicket.Content = dto.Content;

            if (!string.IsNullOrEmpty(dto.Tag))
                existingTicket.Tag = dto.Tag;

            await _ticketRepository.UpdateAsync(existingTicket);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteTicketAsync(Guid ticketId)
        {
            await _ticketRepository.DeleteAsync(ticketId);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> ApproveTicketAsync(Guid ticketId, bool approval, string? note = null)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket != null)
            {
                ticket.Status = approval ? TicketStatus.Approved : TicketStatus.Rejected;
                ticket.Note = note;
                ticket.ApprovedAt = DateTime.UtcNow;
                await _ticketRepository.UpdateAsync(ticket);
            }

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> CloseTicketAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Closed;
                await _ticketRepository.UpdateAsync(ticket);
            }
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Ticket>, int)> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<IPagedResult<Ticket>> GetPagedAsync(
            int page = 1,
            int size = 10,
            Expression<Func<Ticket, bool>>? filter = null,
            Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>? orderBy = null)
        {
            if (filter == null)
            {
                orderBy = t => t.OrderByDescending(t => t.CreatedAt);
            }
            var (items, totalCount) = await _ticketRepository.GetPagedAsync(page, size, filter, orderBy);
            return new PagedResult<Ticket>(items, totalCount, page, size);
        }
    }
}