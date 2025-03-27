using Application.Common.File;
using Application.Common.Pagination;
using Application.DTO;
using Application.Services;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Users;
using Microsoft.AspNetCore.SignalR;
using WibuBlog.Hubs;
using Microsoft.Extensions.Hosting;
using Application.Common.MessageOperations;

namespace WibuBlog.Services
{
    public class UserService(IApiService apiService, IOptions<AuthTokenOptions> authTokenOptions, IHttpContextAccessor httpContextAccessor, FileService fileService, MediaService mediaService, IHubContext<NotificationHub> hubContext)
    {
        private readonly IApiService _apiService = apiService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
		private readonly FileService _fileService = fileService;
        private readonly MediaService _mediaService = mediaService;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        public async Task<UserProfileDto> GetUserProfile()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[_authTokenOptions.Name];
            if (string.IsNullOrEmpty(token))
            {
                return null; 
            }
            var response = await _apiService.GetAsync<ApiResponse<UserProfileDto>>($"User/GetAccountDetails/");
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
        public async Task<PagedResult<User>> GetPagedUsersAsync(int page, int size)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<User>>>($"User/GetPagedUsers?page={page}&size={size}");
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
                NotiType = NotiType.Profile,
                Content = Application.Common.MessageOperations.NotificationService.GetNotification("NOTI08", "Quoc anh"),
                UserId = Guid.Parse(userId),
                IsDeleted = false
            };
            var noti = await apiService.PostAsync<ApiResponse<Notification>>($"Notification/Add/", notification);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Updated Profile");
            var response = await _apiService.PutAsync<ApiResponse<Media>>("User/UpdateProfilePhoto/", media);
            return response.Value!;
		}

		public async Task<HeaderViewDto> GetUserNotifications()
		{
			var response = await _apiService.GetAsync<ApiResponse<HeaderViewDto>>($"User/GetUserNotifications/");
			return response.Value!;
		}
	}
}
