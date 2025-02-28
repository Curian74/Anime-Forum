using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Post;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.Helpers;
using System.Security.Claims;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class PostController(PostServices postService, CommentServices commentServices) : Controller
    {
        private readonly PostServices _postService = postService;
        private readonly CommentServices _commentServices = commentServices;

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _postService.GetPagedPostAsync(page, pageSize, "", "", "", false);
            return View("Index", value);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewPosts([FromQuery] QueryObject queryObject)
        {
            var value = await _postService.GetPagedPostAsync(queryObject.Page, queryObject.Size,
                queryObject.FilterBy, queryObject.SearchTerm, queryObject.OrderBy, queryObject.Descending);

            return View(value);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(Guid id, int? page = 1, int? pageSize = 100)
        {
            var post = await _postService.GetPostByIdAsync(id);
            var comments = await _commentServices.GetPagedComments(page, pageSize);

            var postComments = comments.Items.Where(x => x.PostId == post.Id).ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            PostDetailVM postDetailVM;

            if (userId != null)
            {
                postDetailVM = new PostDetailVM
                {
                    Comments = postComments,
                    Post = post,
                    UserId = Guid.Parse(userId),
                };
            }

            else
            {
                postDetailVM = new PostDetailVM
                {
                    Comments = postComments,
                    Post = post,
                };
            }

            if (post is null)
            {
                return NotFound();
            }

            return View(postDetailVM);
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(string content)
        {
            Console.WriteLine(content);
            return BadRequest(content);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostVM addPostVM)
        {
            if (!ModelState.IsValid)
            {
                return View(addPostVM);
            }

            try
            {
                //var authToken = Request.Cookies[_authTokenOptions.Name];
                await _postService.AddNewPostAsync(addPostVM);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException e)
            {
                ModelState.AddModelError(string.Empty, $"{e.GetType().Name}: {e.Message} {e.StatusCode}");
                return View(addPostVM);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post is null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            try
            {
                var data = await _postService.UpdatePostAsync(id, post);

                return RedirectToAction(nameof(Update), new { id });
            }

            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "*De message loi vao day.*");
                return View(post);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            _ = await _postService.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
