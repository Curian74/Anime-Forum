using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Post;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class PostController(PostService postService, CommentService commentServices,
        PostCategoryService postCategoryService, UserService userService) : Controller
    {
        private readonly PostService _postService = postService;
        private readonly CommentService _commentService = commentServices;
        private readonly PostCategoryService _postCategoryService = postCategoryService;
        private readonly UserService _userService = userService;

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _postService.GetPagedPostAsync(page, pageSize, "", "", "", false);
            return View("Index", value);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewPosts([FromQuery] QueryObject queryObject, Guid? categoryId)
        {
            var postList = await _postService.GetPagedPostAsync(queryObject.Page, queryObject.Size,
                queryObject.FilterBy, queryObject.SearchTerm, queryObject.OrderBy, queryObject.Descending);

            var categoryList = await _postCategoryService.GetAllCategories("" , "" , false);

            var data = new NewPostsVM
            {
                CategoryList = categoryList.OrderBy(x => x.Name).Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToList(),
                Posts = postList,
                CategoryId = categoryId
            };

            return View(data);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(Guid id, int? page = 1, int? pageSize = 10)
        {
            var post = await _postService.GetPostByIdAsync(id);
            var comments = await _commentService
                .GetPagedComments(page, pageSize, "postId", id.ToString());

            //var postComments = comments.Items.Where(x => x.PostId == post.Id).ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserByIdAsync(userId);

            PostDetailVM postDetailVM;

            if (userId != null)
            {
                postDetailVM = new PostDetailVM
                {
                    Comments = comments,
                    Post = post,
                    User = user,
                };
            }

            else
            {
                postDetailVM = new PostDetailVM
                {
                    Comments = comments,
                    Post = post,
                };
            }

            if (post is null)
            {
                return NotFound();
            }

            return View(postDetailVM);
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
