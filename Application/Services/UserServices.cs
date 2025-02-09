using Application.Common.Pagination;
using Application.Interfaces.Pagination;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserServices(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();

        public async Task<IPagedResult<User>> GetPaged(int page, int size = 10)
        {
            var (items, totalCount) = await _userRepository.GetPagedAsync(page, size);
            return new PagedResult<User>(items, totalCount, page, size);
        }
    }
}
