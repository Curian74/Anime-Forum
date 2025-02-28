using Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WibuBlog.Services;
using WibuBlog.ViewModels.Authentication;
namespace WibuBlog.Controllers
{
    public class AuthenticationController(AuthenticationServices authenticationService, IOptions<AuthTokenOptions> authTokenOptions) : Controller
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

        [HttpPost]
        public async Task<IActionResult> LoginAuthentication(LoginVM loginVM)
        {
            var result = await _authenticationService.AuthorizeLogin(loginVM);

            if (!result)
            {
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.AuthorizeLogout();

            return RedirectToAction("Index", "Home");
        }

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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(NotFound());
        }
    }
}
