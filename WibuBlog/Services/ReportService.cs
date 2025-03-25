using Application.Common.Pagination;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
using WibuBlog.Services.Api;
using WibuBlog.ViewModels.Report;
using Application.DTO;
namespace WibuBlog.Services
{
    public class ReportService(IApiService apiService)
    {
        private readonly IApiService _apiService = apiService;

        public async Task<bool> CreateReportAsync(AddReportVM data)
        {
            var response = await _apiService.PostAsync<ApiResponse<Report>>("Report/CreateReport", data);
            return response != null;
        }
        public async Task<List<ReportDto>> GetAllReportWithDetailsAsync(string? filterBy, string? searchTerm, bool? isDesc)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<ReportDto>>>(
                $"Report/GetAllWithDetails?filterBy={filterBy}&searchTerm={searchTerm}&descending={isDesc}");
            return response.Value!;
        }
        
        public async Task<PagedResult<ReportDto>> GetPagedReportWithDetailsAsync(int? page, int? pageSize,
            string? filterBy, string? searchTerm, string? orderBy, bool? isDescending)
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<ReportDto>>>(
                $"Report/GetPagedWithDetails?page={page}&size={pageSize}&filterBy={filterBy}" +
                $"&searchTerm={searchTerm}&orderBy={orderBy}&descending={isDescending}");
            return response.Value!;
        }
    }
}
