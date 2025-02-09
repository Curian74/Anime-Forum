using Application.Common.Pagination;
using Application.DTO;
using Application.Interfaces.Pagination;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class PostServices(IUnitOfWork unitOfWork, IMapper mapper) 
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<Post> _postRepository = unitOfWork.GetRepository<Post>();
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _postRepository.GetAllAsync();
        }

        public async Task<IPagedResult<Post>> GetPagedAsync(int page, int size = 10)
        {
            var (items, totalCount) = await _postRepository.GetPagedAsync(page, size);
            return new PagedResult<Post>(items, totalCount, page, size);
        }

        public async Task<Post> GetByIdAsync(int postId)
        {
            return await _postRepository.GetByIdAsync(postId);
        }

        public async Task<int> CreatePostAsync(PostDto dto)
        {
            var post = _mapper.Map<Post>(dto);

            await _postRepository.AddAsync(post);
            
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> UpdatePostAsync(int postId, PostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Could not find requested post.");

            _mapper.Map(dto, post);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeletePostAsync(int postId)
        {
            _ = await _postRepository.GetByIdAsync(postId) ?? throw new KeyNotFoundException("Could not find requested post.");

            await _postRepository.DeleteAsync(postId);
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
