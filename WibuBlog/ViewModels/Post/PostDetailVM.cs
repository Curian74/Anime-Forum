
using Domain.Entities;

namespace WibuBlog.ViewModels.Post
{
    public class PostDetailVM
    {
        public Domain.Entities.Post? Post { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
