using Domain.Entities;
using WibuBlog.Interfaces.Api;

namespace WibuBlog.Services
{
    public class FlairService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<IEnumerable<UserFlair>> GetUserFlairsAsync(Guid userId)
        {
            return await _apiService.GetAsync<IEnumerable<UserFlair>>($"Inventory/GetUserRepository");
        }

        public async Task<UserFlair> GetActiveFlairAsync(Guid userId)
        {
            return await _apiService.GetAsync<UserFlair>($"Inventory/GetActiveFlair/{userId}");
        }

        public async Task<bool> SetActiveFlairAsync(Guid flairId)
        {
            return await _apiService.PostAsync<bool>($"Inventory/SetActiveFlair/", flairId);
        }
    }
}
