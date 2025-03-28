using Application.DTO;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class AdminService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<WebStatsDto> GetStatsAsync(int days)
        {
            var response = await _apiService.GetAsync<ApiResponse<WebStatsDto>>($"Admin/GetStats?days={days}");
            return response.Value!;
        }
    }
}
