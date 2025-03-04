using Microsoft.AspNetCore.Mvc;
using WibuBlog.Services;

namespace WibuBlog.Views.Shared.ViewComponents
{
    //Dung class nay de gui data den Component tuong ung(Header)
    public class HeaderViewComponent(UserService userService) : ViewComponent
    {   
        private readonly UserService _userService = userService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetUserProfile();
            return View(user);
        }
    }
}
