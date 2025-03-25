using Application.Common.Pagination;

namespace WibuBlog.ViewModels.Report
{
    public class ReportsVM
    {
        public PagedResult<Application.DTO.ReportDto>? Reports { get; set; }
    }
}
