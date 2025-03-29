using Application.Common.Pagination;
using Domain.Entities;
using WibuBlog.Common.ApiResponse;
using WibuBlog.Interfaces.Api;
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

        public async Task<List<ReportDto>> GetAllReportWithDetailsAsync(
            string? filterBy = null,
            string? searchTerm = null,
            bool? isDesc = false,
            Guid? postCategoryId = null)
        {
            var response = await _apiService.GetAsync<List<ReportDto>>(
                $"Report/GetAllWithDetails/WithDetails?filterBy={filterBy}&searchTerm={searchTerm}&descending={isDesc}&PostCategoryId={postCategoryId}"); // Thêm PostCategoryId vào query
            return response ?? new List<ReportDto>();
        }

        public async Task<PagedResult<ReportDto>> GetPagedReportWithDetailsAsync(
            int? page,
            int? pageSize,
            string? filterBy,
            string? searchTerm,
            string? orderBy,
            bool? isDescending,
            Guid? postCategoryId = null) 
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResult<ReportDto>>>(
                $"Report/GetPagedWithDetails?page={page}&size={pageSize}&filterBy={filterBy}" +
                $"&searchTerm={searchTerm}&orderBy={orderBy}&descending={isDescending}&PostCategoryId={postCategoryId}"); 
            return response.Value!;
        }

        public async Task<bool> ApproveReportAsync(Guid reportId, bool approval, string? note = null)
        {
            var response = await _apiService.PutAsync<ApiResponse<Report>>(
                $"Report/ApproveReport/reports/{reportId}",
                new ApproveReportDto
                {
                    Approval = approval,
                    Note = note
                }
            );
            return response != null;
        }

    }

}
