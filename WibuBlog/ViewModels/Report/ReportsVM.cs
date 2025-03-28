using Application.Common.Pagination;
using Application.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.ViewModels.Report
{
    public class ReportsVM
    {
        public PagedResult<ReportDto> Reports { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }
        public Guid PostCategoryId { get; set; }
    }
}
