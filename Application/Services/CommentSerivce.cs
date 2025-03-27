
using Application.Common.Pagination;
using Application.DTO;
using Application.DTO.Comment;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services
{
    public class CommentSerivce(IUnitOfWork unitOfWork, IMapper mapper)
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

        public async Task<int> PostCommentAsync(PostCommentDto dto)
        {
            var comment = _mapper.Map<Comment>(dto);

            await _commentGenericRepository.AddAsync(comment);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdateCommentAsync(Guid commentId, EditCommentDto dto)
        {
            var comment = await _commentGenericRepository.GetByIdAsync(commentId) ?? throw new KeyNotFoundException("Could not find requested post.");

            _mapper.Map(dto, comment);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentGenericRepository.GetByIdAsync(commentId) ?? throw new KeyNotFoundException("Could not find requested post.");

            comment.IsHidden = true;

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
