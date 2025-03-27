
using Application.Common.Pagination;
using Application.Interfaces.Pagination;
using Domain.Entities;
using System.Linq.Expressions;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.ViewModels.Comment;

namespace WibuBlog.Services
{
    public class CommentService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<PagedResult<Comment>> GetPagedComments(int? page, int? pageSize, string? filterBy, string? searchTerm, string? orderBy, bool? descending)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<Comment>>>
                ($"Comment/GetPaged?page={page}&size={pageSize}" +
                $"&filterBy={filterBy}&searchTerm={searchTerm}&orderBy={orderBy}&descending={descending}");

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
