using WibuBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WibuBlog.ViewModels.Users;
using Application.Common.MessageOperations;
using WibuBlog.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Application.Common.Pagination;
using Domain.Entities;
using System.Security.Claims;

namespace WibuBlog.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "MemberPolicy")]
    public class UserController(UserService userService, IWebHostEnvironment webHostEnvironment, RankService rankService) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly RankService _rankService = rankService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid? userId = null)
        {
            var result = await _userService.GetUserProfile(userId);

            if (result is null)
            {
                return NotFound();
            }

            return View(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminPolicy")]
        public async Task<IActionResult> UserList(UserQueryVM? query)
        {
            var usersList = await _userService.GetAllAsync("", "", false);

            var ranksList = await _rankService.GetAllAsync("", "", "name", false);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var filteredUsers = usersList.Where(x => x.Id != Guid.Parse(currentUserId));

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                filteredUsers = filteredUsers.Where(x => x.UserName
                .Contains(query.SearchTerm.Trim()) || x.Email
                .Contains(query.SearchTerm.Trim())).ToList();
            }

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                filteredUsers = filteredUsers.OrderByDescending(x => x.Points);
            }

                if (query.SelectedRankId.HasValue)
            {
                filteredUsers = filteredUsers.Where(x => x.RankId == query.SelectedRankId).ToList();
            }

            //filteredUsers = filteredUsers.Where(x => x.IsActive == query.IsInactive).ToList();


            filteredUsers = filteredUsers.Where(x => x.IsBanned == query.IsBanned).ToList();

            int totalItems = filteredUsers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
            int skip = (query.Page - 1) * query.PageSize;

            var pagedUsers = filteredUsers.Skip(skip).Take(query.PageSize).ToList();

            var userListVM = new UserListVM
            {
                UsersList = new PagedResult<User>(pagedUsers, filteredUsers.Count(), query.Page, query.PageSize),
                RanksList = ranksList.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList(),
            };

            return View(userListVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUserVM updateUserVM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UserProfile));
            }

            var user = await _userService.UpdateUserAsync(updateUserVM);
            return RedirectToAction(nameof(UserProfile));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordVM model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(MessageConstants.ME006);
            }
            var response = await _userService.UpdatePassword(model);
            if (response.Succeeded)
            {
                return Ok(MessageConstants.ME007a);
            }
            return BadRequest(response.Errors[0].Description);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePhoto(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.UpdateProfilePhoto(file,userId);

            return RedirectToAction(nameof(UserProfile));
        }

    }
}
