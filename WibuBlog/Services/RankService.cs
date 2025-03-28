using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class RankService
    {
        private readonly IApiService _apiService;

        public RankService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<Rank>> GetAllAsync(string? filterBy, string? searchTerm, string orderBy, bool? isDesc)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<Rank>>>(
                $"Rank/GetAll?filterBy={filterBy}&orderBy={orderBy}&searchTerm={searchTerm}&descending={isDesc}");
            return response.Value!;
        }
    }
}
