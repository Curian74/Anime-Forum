
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Services
{
    public class PostCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<PostCategory> _postCategoryRepository = unitOfWork.GetRepository<PostCategory>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<PostCategory> Items, int TotalCount)> GetAllAsync(
            Expression<Func<PostCategory, bool>>? filter = null,
            Func<IQueryable<PostCategory>, IOrderedQueryable<PostCategory>>? orderBy = null)
        {
            return await _postCategoryRepository.GetAllAsync(filter, orderBy);
        }
    }
}
