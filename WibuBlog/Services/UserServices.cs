using Application.Common.Pagination;
using Application.DTO;
using Domain.Entities;
using Infrastructure.Common.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class UserServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;
        public async Task<User> GetUserByIdAsync<T>(T userId)
        {
            var response = await _apiService.GetAsync<ApiResponse<User>>($"User/GetUserById/{userId}");
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
