using Application.Common.Pagination;
using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces.Email;
using System.Reflection;
using Application.Common.Validations;
namespace Application.Services
{
    public class UserService(UserManager<User> userManager, IMapper mapper, IUnitOfWork unitOfWork, UserEditFieldValidations userEditFieldValidations)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IGenericRepository<User> _userGenericRepository = unitOfWork.GetRepository<User>();
        private readonly UserEditFieldValidations _userEditFieldValidations = userEditFieldValidations;

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

        public async Task<User?> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
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

        public async Task UpdateByFieldAsync(EditUserDto targetEditUser, Guid currrentUserId)
        {          
            var targetUser = await _userGenericRepository.GetByIdAsync(targetEditUser.userId);
            var currentUser = await _userGenericRepository.GetByIdAsync(currrentUserId);
            var currentRole = (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault().ToString();        
            if (_userEditFieldValidations.IsAllowed(targetEditUser,currentRole,currentUser.Id))
            {
                PropertyInfo property = targetUser.GetType().GetProperty(targetEditUser.field);
                if (property == null || !property.CanWrite) throw new ArgumentException($"Field '{targetEditUser.field}' does not exist or cannot be written.");
                object convertedValue = Convert.ChangeType(targetEditUser.value, property.PropertyType);
                property.SetValue(targetUser, convertedValue);
                await _userGenericRepository.UpdateAsync(targetUser);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}
