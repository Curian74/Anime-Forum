using Application.DTO;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class VoteService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<Vote?> GetCurrentUserVoteAsync(Guid postId)
        {
            var response = await _apiService.GetAsync<ApiResponse<Vote?>>($"Vote/GetCurrentUserVote/{postId}");
            return response.Value!;
        }

        public async Task<int> GetTotalPostVoteAsync(Guid postId)
        {
            var response = await _apiService.GetAsync<ApiResponse<int>>($"Vote/GetTotalPostVote/{postId}");
            return response.Value!;
        }

        public async Task<int> ToggleVoteAsync(VoteDto dto)
        {
            var response = await _apiService.PostAsync<ApiResponse<int>>($"Vote/ToggleVote/", dto);
            return response.Value;
        } 
    }
}
