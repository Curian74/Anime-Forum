using Application.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;

namespace WibuBlog.Views.Shared.ViewComponents
{
	public class HeaderViewComponent(UserService userService) : ViewComponent
	{
		private readonly UserService _userService = userService;

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = await _userService.GetUserProfile();
			

            if (user != null)
			{
                var model = await _userService.GetUserNotifications();
                return View(model);
            }
			return View(new HeaderViewDto());	      
		}
	}
}
