using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class AdminService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<User> _userGenericRepository = unitOfWork.GetRepository<User>();
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetAllUsersAsync()
        {
            return await _userGenericRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userGenericRepository.GetByIdAsync(userId);
        }

        public async Task<int?> UpdateUserAsync(Guid userId, UserProfileDto dto)
        {
            var user = await _userGenericRepository.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException("Could not find requested user.");

            _mapper.Map(dto, user);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int?> DeleteUserAsync(Guid userId)
        {

            _ = await _userGenericRepository.GetByIdAsync(userId)
                       ?? throw new KeyNotFoundException("Could not find requested user.");

            await _userGenericRepository.DeleteAsync(userId);

            return await _unitOfWork.SaveChangesAsync();
        }


    }
}
