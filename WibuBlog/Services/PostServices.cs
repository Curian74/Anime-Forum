using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class PostServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<PagedResult<Post>> GetPagedPostAsync(int page, int pageSize)
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

        public async Task<bool> AddNewPostAsync(object data)
        {
            var response = await _apiService.PostAsync<ApiResponse<object>>("Post/Create", data);
            return response != null;
        }

        public async Task<bool> UpdatePostAsync<T>(T id, object data)
        {
            var response = await _apiService.PutAsync<ApiResponse<object>>($"Post/Update/{id}", data);
            return response != null;
        }

        public async Task<bool> DeletePostAsync<T>(T id)
        {
            var response = await _apiService.DeleteAsync($"Post/Delete/{id}");

            return response;
        }
    }
}
