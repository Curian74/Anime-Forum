using Application.Common.Pagination;
using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Email;

namespace Application.Services
{
    public class UserServices(UserManager<User> userManager, IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailServices = emailService;
        private readonly IGenericRepository<User> _userGenericRepository = unitOfWork.GetRepository<User>();

        public async Task<User?> FindByLoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Login);

            user ??= await _userManager.FindByEmailAsync(dto.Login);

            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<IdentityResult> Register(RegisterDto dto) 
        {
            var user = new User { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
            }

            return result;
        }

        public async Task<bool> Login(LoginDto dto)
        {
            var user = await this.FindByLoginAsync(dto);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);

            return result;
        }

        public async Task<UserProfileDto?> GetProfileDetails(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            var result = _mapper.Map<UserProfileDto>(user);

            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }
        public async Task<User?> FindByIdAsync(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }


        public async Task<PagedResult<User>> GetPagedUsersAsync(int page, int size)
        {
            var (items, totalCount) = await _userGenericRepository.GetPagedAsync(page, size);
            return new PagedResult<User>(items, totalCount, page, size);
        }

    }
}
