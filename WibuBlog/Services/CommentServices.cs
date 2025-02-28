
using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class CommentServices(IApiServices apiService)
    {
        private readonly IApiServices _apiService = apiService;

        public async Task<PagedResult<Comment>> GetPagedComments(int? page, int? pageSize)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Comment>>>
                ($"Comment/GetPaged?page={page}&size={pageSize}");

            return response.Value!;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<Comment>>>("Comment/GetAll");

            return response.Value!;
        }
    }
}
