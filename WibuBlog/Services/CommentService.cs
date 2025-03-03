
using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Comment;
using WibuBlog.ViewModels.Post;

namespace WibuBlog.Services
{
    public class CommentService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<PagedResult<Comment>> GetPagedComments(int? page, int? pageSize, string? filterBy, string? searchTerm)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Comment>>>
                ($"Comment/GetPaged?page={page}&size={pageSize}&filterBy={filterBy}&searchTerm={searchTerm}");

            return response.Value!;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<Comment>>>("Comment/GetAll");

            return response.Value!;
        }

        public async Task<bool> PostCommentAsync(PostCommentVM commentVM)
        {
            var response = await _apiService.PostAsync<ApiResponse<Post>>("Comment/PostComment", commentVM);
            return response != null;
        }
    }
}
