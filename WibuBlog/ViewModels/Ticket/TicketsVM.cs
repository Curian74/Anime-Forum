using Application.Common.Pagination;
using Application.Interfaces.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.ViewModels.Ticket
{
    public class TicketsVM
    {
        public IPagedResult<Domain.Entities.Ticket> Tickets { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }
        public Guid PostCategoryId { get; set; }
    }
}
