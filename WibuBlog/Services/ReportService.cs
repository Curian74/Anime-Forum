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

        public async Task<bool> CreateReportAsync(AddReportVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Post>>("Report/CreateReport", data);
            return response != null;
        }
    }
}
