using Application.Common.Pagination;
using Application.DTO;
using Application.DTO.Post;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services
{
    public class PostService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
        private readonly IGenericRepository<PostCategory> _categoryRepository = unitOfWork.GetRepository<PostCategory>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<Post> Items, int TotalCount)> GetAllAsync(
            Expression<Func<Post, bool>>? filter = null,
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null)
        {
            return await _postRepository.GetAllAsync(filter, orderBy);
        }

        public async Task<(IEnumerable<Post>, int totalCount)> FindAsync(
            Expression<Func<Post, bool>>? filter = null,
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null)
        {
            return await _postRepository.GetPagedAsync(1, 10, filter, orderBy);
        }

        public async Task<IPagedResult<Post>> GetPagedAsync(
            int page = 1,
            int size = 10,
            Expression<Func<Post, bool>>? filter = null,
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null)
        {
            var (items, totalCount) = await _postRepository.GetPagedAsync(page, size, filter, orderBy);
            return new PagedResult<Post>(items, totalCount, page, size);
        }

        public async Task<Post?> GetByIdAsync(Guid postId)
        {
            return await _postRepository.GetByIdAsync(postId);
        }

        public async Task<int> CreatePostAsync(CreatePostDto dto)
        {
            var post = _mapper.Map<Post>(dto);

            await _postRepository.AddAsync(post);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdatePostAsync(Guid postId, CreatePostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Could not find requested post.");

            _mapper.Map(dto, post);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeletePostAsync(Guid postId)
        {
            _ = await _postRepository.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Could not find requested post.");

            await _postRepository.DeleteAsync(postId);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeletePostWhereAsync(Expression<Func<Post, bool>> filter)
        {
            await _postRepository.DeleteWhereAsync(filter);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeactivatePostAsync(Guid postId, DeactivatePostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Could not find requested post.");

            post.IsHidden = dto.IsHidden;

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
