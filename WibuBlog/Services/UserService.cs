using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class UserService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;
        public async Task<User> GetUserByIdAsync<T>(T userId)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserById/{userId}");
            return response.Value!;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserByEmail?email={email}");
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
