using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class PostCategoryServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<List<PostCategory>> GetAllCategories(string? filterBy, string? searchTerm, bool? isDesc)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<PostCategory>>>(
                $"PostCategory/GetAll?filterBy={filterBy}&searchTerm={searchTerm}&descending={isDesc}");
            return response.Value!;
        }
    }
}
