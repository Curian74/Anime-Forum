﻿using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Post;

namespace WibuBlog.Services
{
    public class PostServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<PagedResult<Post>> GetPagedPostAsync(int? page, int? pageSize)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Post>>>(
                $"Post/GetPaged?page={page}&size={pageSize}");

            return response.Value!;
        }

        public async Task<Post> GetPostByIdAsync<T>(T id)
        {
            var response = await _apiService.GetAsync<ApiResponse<Post>>($"Post/Get/{id}");
            return response.Value!;
        }

        public async Task<bool> AddNewPostAsync(AddPostVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Post>>("Post/Create", data);
            Console.WriteLine(response.Value);
            return response != null;
        }

        public async Task<Post> UpdatePostAsync<T>(T id, Post data)
        {
            var response = await _apiService.PutAsync<ApiResponse<Post>>($"Post/Update/{id}", data);
            return response.Value!;
        }

        public async Task<bool> DeletePostAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Post/Delete/{id}");

            return response;
        }
    }
}
