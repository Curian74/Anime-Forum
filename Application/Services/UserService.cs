﻿using Application.Common.File;
using Application.Common.Pagination;
using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService(UserManager<User> userManager, IMapper mapper, IUnitOfWork unitOfWork, RankService rankService)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IGenericRepository<User> _userGenericRepository = unitOfWork.GetRepository<User>();
        private readonly IGenericRepository<Post> _postGenericRepository = unitOfWork.GetRepository<Post>();
        private readonly RankService _rankService = rankService;

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

        public async Task<UserProfileDto?> GetProfileDetails(Guid? id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            await _rankService.UpdateUserRankAsync(id);

            var result = _mapper.Map<UserProfileDto>(user);
            result.PostList = await _postGenericRepository.GetAllWhereAsync(m => m.UserId == id);

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

        public async Task<int> UpdateUserAsync(UpdateUserDto userUpdateDTO)
        {
            var updateUser = await _userManager.FindByIdAsync(userUpdateDTO.Id.ToString());
            _mapper.Map(userUpdateDTO,updateUser);          
            return await _unitOfWork.SaveChangesAsync();
        }

		public async Task<IdentityResult> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO)
		{
			var updateUser = await _userManager.FindByIdAsync(updatePasswordDTO.UserId.ToString());
            return await _userManager.ChangePasswordAsync(updateUser, updatePasswordDTO.OldPassword, updatePasswordDTO.NewPassword);
		}

		public async Task<Media> UpdateProfilePhotoAsync(Media media, string currentUserId)
        {
            var updateUser = await _userManager.FindByIdAsync(currentUserId);
            updateUser.ProfilePhoto = media;
            await _unitOfWork.SaveChangesAsync();
            return media;
		}
	}
}
