using Application.DTO;
using Domain.Entities;
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

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            var response = await _apiService.GetAsync<ApiResponse<IEnumerable<Ticket>>>("Admin/GetAllTickets");
            return response.Value!;
        }
    }
}
