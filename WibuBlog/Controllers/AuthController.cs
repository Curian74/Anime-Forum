using Application.Common.EmailTemplate;
using Application.Common.MessageOperations;
using Infrastructure.Extensions;
using Application.Common.Session;
using Application.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WibuBlog.Services;
using WibuBlog.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WibuBlog.Controllers
{
    public class AuthController(AuthenticationService authenticationService, IEmailService emailService, UserService userService, OTPValidation OTPValidation) : Controller
    {
        private readonly AuthenticationService _authenticationService = authenticationService;
        private readonly IEmailService _emailService = emailService;
        private readonly UserService _userService = userService;
        private readonly OTPValidation _OTPValidation = OTPValidation;

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
        public async Task<ActionResult> RegisterAuthentication(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(nameof(Register), registerVM);

            if (await _userService.GetUserByEmailAsync(registerVM.email) != null)
            {
                ModelState.AddModelError("email", MessageConstants.MEN004);
                return View(nameof(Register), registerVM);
            }
            string otp = OTPGenerator.GenerateOTP();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(5).ToString());
            string registerData = JsonConvert.SerializeObject(registerVM);
            HttpContext.Session.SetString("RegisterVM", registerData);
            var model = new Dictionary<string, string>
                {
                    { "Name", registerVM.username },
                    { "OTP", otp  }
                };
            _emailService.SendEmailAsync(registerVM.email, "Registration OTP", EmailTemplateEnum.Registration, model);

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

            Dictionary<string, string> errors = _OTPValidation.ValidateOTP(savedOtp, OTP, expiryTimeStr);

            if (errors.Count == 0)
            {
                RegisterVM registerVM = JsonConvert.DeserializeObject<RegisterVM>(registerData);
                HttpContext.Session.Clear();
                var result = await _authenticationService.AuthorizeRegister(registerVM);
                TempData["SuccessRegistrationMessage"] = MessageConstants.MEN003;
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
    }
}
