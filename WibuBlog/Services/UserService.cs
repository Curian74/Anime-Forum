using Application.Common.File;
using Application.Common.Pagination;
using Application.DTO;
using Application.Services;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Application.Common.MessageOperations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace WibuBlog.Services
{
    public class UserService(IApiService apiService, IOptions<AuthTokenOptions> authTokenOptions, IHttpContextAccessor httpContextAccessor, FileService fileService, MediaService mediaService)
    {
        private readonly IApiService _apiService = apiService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly FileService _fileService = fileService;
        private readonly MediaService _mediaService = mediaService;

		public async Task<UserProfileDto> GetUserProfile(Guid? userId = null)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_authTokenOptions.Name];
            if (string.IsNullOrEmpty(token))
            {
                return null; 
            }
            var response = await _apiService.GetAsync<ApiResponse<UserProfileDto>>($"User/GetAccountDetails/{userId}");
            return response.Value!;
        }

		public async Task<UserProfileDto> GetMemberProfile(Guid? userId = null)
		{
			var response = await _apiService.GetAsync<ApiResponse<UserProfileDto>>($"User/GetAccountDetails?userId={userId}");
            return response.Value!;
		}
		public async Task<User> GetUserById(Guid userId)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserById/{userId}");
            return response.Value!;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserByEmail?email={email}");
            return response.Value!;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserByUsername?username={username}");
            return response.Value!;
        }

        public async Task<PagedResult<User>> GetPagedUserAsync(int? page, int? pageSize, string? filterBy, string? searchTerm, string? orderBy, bool? descending)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<User>>>
                ($"User/GetPaged?page={page}&size={pageSize}" +
                $"&filterBy={filterBy}&searchTerm={searchTerm}&orderBy={orderBy}&descending={descending}");

            return response.Value!;
        }

        public async Task<List<User>> GetAllAsync(string? filterBy, string? searchTerm, bool? isDesc)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<User>>>(
                $"User/GetAll?filterBy={filterBy}&searchTerm={searchTerm}&descending={isDesc}");
            return response.Value!;
        }

        public async Task<UpdateUserDto> UpdateUserAsync(UpdateUserVM updateUserVM)
        {
            UpdateUserDto updateUserDto = new UpdateUserDto()
            {
               Id = updateUserVM.UserId,
               Bio = updateUserVM.Bio,
               PhoneNumber = updateUserVM.Phone
            };
            var response = await _apiService.PutAsync<ApiResponse<UpdateUserDto>>($"User/UpdateUser/", updateUserDto);
            return response.Value!;
        }

        public async Task<ApiResponse<IdentityResult>> UpdatePassword(UpdatePasswordVM model)
        {
			UpdatePasswordDTO updatePasswordDTO = new UpdatePasswordDTO()
			{
                UserId = model.UserId,
				OldPassword = model.OldPassword,
				NewPassword = model.NewPassword,
				ConfirmPassword = model.ConfirmPassword
			};
			var response = await _apiService.PutAsync<ApiResponse<IdentityResult>>($"User/UpdatePassword/", updatePasswordDTO);
			return response;
		}

		public async Task<Media> UpdateProfilePhoto(IFormFile file, string userId)
        {
            Media media = await _fileService.UploadImage(file);
            //await _fileService.DeleteCurrentProfilePhoto();
            var resp = await _apiService.PostAsync<ApiResponse<Media>>($"Media/Add/", media);
            Notification notification = new Notification()
            {
                Content = Application.Common.MessageOperations.NotificationService.GetNotification("NOTIN01"),
                UserId = Guid.Parse(userId),
                IsDeleted = false,
            };
            var noti = await apiService.PostAsync<ApiResponse<Notification>>($"Notification/Add/", notification);      
            var response = await _apiService.PutAsync<ApiResponse<Media>>("User/UpdateProfilePhoto/", media);
            return response.Value!;
		}

		public async Task<HeaderViewDto> GetUserNotifications()
		{
			var response = await _apiService.GetAsync<ApiResponse<HeaderViewDto>>($"User/GetUserNotifications/");
			return response.Value!;
		}

        public async Task<User> GetUserByEmail(string email)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserByEmail?email={email}");
            return response.Value!;
        }


        public async Task<UserProfileDto> GetMemberInfo(string userId)
        {
            var response = await _apiService.GetAsync<ApiResponse<UserProfileDto>>($"User/GetMemberProfile?userId={userId}");
            return response.Value!;
        }


        public async Task<bool> ToggleModeratorRoleAsync(Guid userId)
        {
            var response = await _apiService.PostAsync<ApiResponse<bool>>($"Admin/ToggleModeratorRole/", userId);
            return response.Value!;
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<string>>>($"User/GetUserRoles/{userId}");

            return response?.Value ?? [];
        }

    }
}
