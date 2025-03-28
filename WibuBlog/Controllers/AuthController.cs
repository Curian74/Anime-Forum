using Application.Common.MessageOperations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WibuBlog.Services;
using WibuBlog.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlog.Controllers
{
    public class AuthController(AuthService authenticationService, UserService userService, OtpService otpService) : Controller
    {
        private readonly AuthService _authenticationService = authenticationService;
        private readonly UserService _userService = userService;
        private readonly OtpService _otpService = otpService;

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
                TempData["LoginRequired"] = MessageConstants.MEN012;
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
        public async Task<ActionResult> RegisterAuthentication(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Register), registerVM);
            }

            if (await _userService.GetUserByEmailAsync(registerVM.email) != null)
            {
                ModelState.AddModelError("email", MessageConstants.MEN004);
                return View(nameof(Register), registerVM);
            }

            await _otpService.SendOtp(registerVM);

            return View(nameof(OTPAuthentication));
        }


        [HttpPost]
        public async Task<ActionResult> OTPAuthentication(string OTP)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(OTPAuthentication));
            }

            string savedOtp = HttpContext.Session.GetString("OTP");
            string expiryTimeStr = HttpContext.Session.GetString("OTP_Expiry");
            string registerData = HttpContext.Session.GetString("RegisterVM");

            var errors = _otpService.ValidateOTP(savedOtp, OTP, expiryTimeStr);

            if (errors.Count == 0)
            {
                RegisterVM registerVM = JsonConvert.DeserializeObject<RegisterVM>(registerData);
                HttpContext.Session.Clear();
                var result = await _authenticationService.AuthorizeRegister(registerVM);
                TempData["RegistrationSuccess"] = MessageConstants.MEN003;
                return RedirectToAction(nameof(Login));
            }
            foreach (var x in errors)
            {
                ModelState.AddModelError(x.Key, x.Value);
            }
            return View(nameof(OTPAuthentication));
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(NotFound());
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (_userService.GetUserByEmail(email) == null)
            {
                TempData["ErrorMessage"] = MessageConstants.ME006;
                return RedirectToAction(nameof(ForgotPassword));
            }
            return View("ForgotPasswordConfirmation");
        }
    }
}
