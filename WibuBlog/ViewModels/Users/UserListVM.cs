using Application.Common.Pagination;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.ViewModels.Users
{
    public class UserListVM
    {
        public PagedResult<User>? UsersList { get; set; }
        public List<SelectListItem>? RanksList { get; set; }
        public Guid? SelectedRankId { get; set; }
    }
}
