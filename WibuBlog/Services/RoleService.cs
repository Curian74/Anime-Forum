using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class RoleService
    {
        private readonly IApiService _apiService;

        public RoleService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<string>> GetUserRoleAsync(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<string>>>($"Role/GetUserRole/{id}");
            return response.Value!;
        }
    }
}
