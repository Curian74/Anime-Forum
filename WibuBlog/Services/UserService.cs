using Application.Common.Pagination;
using Application.DTO;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class UserService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;
        public async Task<UserProfileDto> GetUserProfile()
        {
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

        public async Task<User> UpdateUserAsync<T>(T userId, User data)
        {
            var response = await _apiService.PutAsync<ApiResponse<User>>($"User/UpdateUser/{userId}", data);
            return response.Value!;
        }

    }
}
