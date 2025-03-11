using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.Services.Api;
using WibuBlog.ViewModels.Report;

namespace WibuBlog.Services
{
    public class ReportService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        // In ReportService.cs
        public async Task<bool> CreateReportAsync(AddReportVM data)
        {
            // Log the data being sent
            var response = await _apiService.PostAsync<ApiResponse<Post>>("Report/CreateReport", data);
            Console.WriteLine($"Response: {response?.Value}");
            return response != null;
        }
    }
}
