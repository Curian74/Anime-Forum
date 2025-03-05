using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;
using WibuBlog.ViewModels.Post;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Application.Common.Pagination;
using System.Threading.Tasks;

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

        private async Task<List<SelectListItem>> GetCategoryListAsync()
        {
            var categories = await _postCategoryService.GetAllCategories("", "", false);
            return categories.OrderBy(c => c.Name)
                             .Select(c => new SelectListItem
                             {
                                 Text = c.Name,
                                 Value = c.Id.ToString()
                             }).ToList();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page = 1, int? pageSize = 5)
        {
            var value = await _postService.GetPagedPostAsync(page, pageSize, "", "", "", false);
            return View("Index", value);
        }

        [AllowAnonymous]
        public async Task<IActionResult> NewPosts([FromQuery] QueryObject queryObject, Guid? postCategoryId)
        {
            var postList = await _postService.GetAllPostAsync("", "", false);

            var filteredPosts = postCategoryId.HasValue
                ? postList.Where(x => x.PostCategoryId == postCategoryId)
                : postList;

            if (!string.IsNullOrEmpty(queryObject.SearchTerm))
            {
                filteredPosts = filteredPosts.Where(x => x.Title.Contains(queryObject.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            int totalItems = filteredPosts.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryObject.Size);
            int skip = (queryObject.Page - 1) * queryObject.Size;

            var pagedPosts = filteredPosts.Skip(skip).Take(queryObject.Size).ToList();

            var categoryList = await GetCategoryListAsync();

            var data = new NewPostsVM
            {
                CategoryList = categoryList,
                Posts = new PagedResult<Post>(pagedPosts, totalItems, queryObject.Page, queryObject.Size),
                PostCategoryId = postCategoryId
            };

            return View(data);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(Guid id, int? page = 1, int? pageSize = 10)
        {
            var post = await _postService.GetPostByIdAsync(id);
            var comments = await _commentService
                .GetPagedComments(page, pageSize, "postId", id.ToString(), "createdAt", true);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = null;
            PostDetailVM postDetailVM;

            if (userId != null)
            {
                user = await _userService.GetUserById(Guid.Parse(userId));
            }

            postDetailVM = new PostDetailVM
            {
                Comments = comments,
                Post = post,
                User = user,
            };

            if (post is null)
            {
                return NotFound();
            }

            return View(postDetailVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var postCategories = await _postCategoryService.GetAllCategories("isRestricted", "false", false);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = null;

            if (userId != null)
            {
                user = await _userService.GetUserById(Guid.Parse(userId));
            }

            var data = new CreatePostVM
            {
                CategoryList = postCategories.OrderBy(p => p.Name).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList(),
                User = user
            };

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM createPostVM)
        {
            var postCategoryList = await _postCategoryService.GetAllCategories("isRestricted", "false", false);
            if (!ModelState.IsValid)
            {
                createPostVM.CategoryList = postCategoryList.OrderBy(p => p.Name).Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
                return View(createPostVM);
            }

            try
            {
                await _postService.CreatePostAsync(createPostVM);
                TempData["successMessage"] = "Post created successfully.";
                return RedirectToAction(nameof(Create));
            }

            catch (Exception ex)
            {
                TempData["successMessage"] = $"Failed to create post. Error: {ex.Message}";
                return View(createPostVM);
            }
        }

        #region TestRoutes
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
        #endregion
    }
}
