using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Comment;

namespace WibuBlog.Controllers
{
    public class CommentController(CommentService commentService) : Controller
    {
        private readonly CommentService _commentService = commentService;

        [HttpPost]
        public async Task<IActionResult> PostComment(PostCommentVM commentVM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Detail", "Post", new { id = commentVM.PostId });
            }
            try
            {
                await _commentService.PostCommentAsync(commentVM);
            }

            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
            }

            return RedirectToAction("Detail", "Post", new { id = commentVM.PostId });
        }
    }
}
