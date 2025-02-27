using Application.Common.MessageOperations;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WibuBlog.Services;
using WibuBlog.ViewModels;
using WibuBlog.ViewModels.Authentication;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace WibuBlog.Controllers
{
    public class AuthenticationController(AuthenticationServices authenticationService,IOptions<AuthTokenOptions> authTokenOptions) : Controller
    {
        private readonly AuthenticationServices _authenticationService = authenticationService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OTPAuthentication()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> LoginAuthentication([FromBody] LoginVM loginVM)
        //{
        //    _ = await _authenticationService.AuthorizeLogin(loginVM);
        //    Request.Cookies.TryGetValue(_authTokenOptions.Name, out string? authToken);
        //    if (authToken is null)
        //    {
        //        return RedirectToAction("Login", "Authentication");
        //    }

        //    Response.Cookies.Append("AuthToken", token, cookieOptions);

        //    return RedirectToAction("Index", "Home");
        //}

        [HttpPost]
        public async Task<IActionResult> RegisterAuthentication(RegisterVM registerVM)
        {

            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var result = await _authenticationService.AuthorizeRegister(registerVM);
            if (result is null)
            {
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }


    }
}
