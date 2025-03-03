using Application.Common.Pagination;
using Domain.Entities;

namespace WibuBlog.ViewModels.Post
{
    public class PostDetailVM
    {
        public Domain.Entities.Post? Post { get; set; }
        public PagedResult<Domain.Entities.Comment>? Comments { get; set; }
        public User? User {  get; set; }
    }
}
