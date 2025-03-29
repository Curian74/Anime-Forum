using Application.Common.Pagination;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.ViewModels.Post
{
    public class NewPostsVM
    {
        public PagedResult<Domain.Entities.Post>? Posts { get; set; }
        public Guid? PostCategoryId { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }
        public User? User { get; set; }
    }
}
