using Microsoft.AspNetCore.Mvc;

namespace WibuBlog.Views.Shared.ViewComponents
{
    //Dung class nay de gui data den Component tuong ung(Header)
    public class HeaderViewComponent : ViewComponent
    {   
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Data can gui se viet o day
            return View();
        }
    }
}
