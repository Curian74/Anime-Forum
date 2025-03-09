using Application.Common.Pagination;
using Application.DTO;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Users;

namespace WibuBlog.Services
{
    public class UserService(IApiService apiService, IOptions<AuthTokenOptions> authTokenOptions, IHttpContextAccessor httpContextAccessor)
    {
        private readonly IApiService _apiService = apiService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
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
            UpdateUserDto editUserDto = new UpdateUserDto()
            {
               userId = updateUserVM.userId,
               bio = updateUserVM.bio,
               phone = updateUserVM.phone,
               password = updateUserVM.password,
            };
            var response = await _apiService.PutAsync<ApiResponse<UpdateUserDto>>($"User/UpdateUser/", editUserDto);
            return response.Value!;
        }
    }
}
