
using Application.Common.Pagination;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services
{
    public class CommentSerivces(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Comment> _commentGenericRepository = unitOfWork.GetRepository<Comment>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<Comment> Items, int TotalCount)> GetAllAsync(
            Expression<Func<Comment, bool>>? filter = null,
            Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null)
        {
            return await _commentGenericRepository.GetAllAsync(filter, orderBy);
        }

        public async Task<IPagedResult<Comment>> GetPagedAsync(
           int page = 1,
           int size = 10,
           Expression<Func<Comment, bool>>? filter = null,
           Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null)
        {
            var (items, totalCount) = await _commentGenericRepository.GetPagedAsync(page, size, filter, orderBy);
            return new PagedResult<Comment>(items, totalCount, page, size);
        }
    }
}
