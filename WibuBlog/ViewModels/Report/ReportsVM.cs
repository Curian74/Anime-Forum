using Application.Common.Pagination;
using Application.DTO;

namespace WibuBlog.ViewModels.Report
{
    public class ReportsVM
    {
        public PagedResult<ReportDto> Reports { get; set; }
        public List<MostReportedPostDto> MostReportedPosts { get; set; }
    }

    public class MostReportedPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int ReportCount { get; set; }
    }

}
