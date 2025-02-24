using Application.DTO;
using Domain.Entities;
using Infrastructure.Common.ApiResponse;
using Microsoft.AspNetCore.Mvc;
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

    }
}
