using Application.Common.EmailTemplate;
using Application.Common.MessageOperations;
using Application.Common.OTPGenerator;
using Application.Common.Session;
using Application.DTO;
using Application.Interfaces.Email;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using WibuBlog.Services;
using WibuBlog.ViewModels.Authentication;
namespace WibuBlog.Controllers
{
    public class AuthenticationController(AuthenticationServices authenticationService,IOptions<AuthTokenOptions> authTokenOptions,
        IHttpClientFactory httpClientFactory, IEmailService emailService, UserServices userServices, OTPValidation OTPValidation) : Controller
     {
        private readonly AuthenticationServices _authenticationService = authenticationService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly IEmailService _emailService = emailService;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly UserServices _userServices = userServices;
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
        public async Task<IActionResult> LoginAuthentication(LoginVM loginVM)
        {
          //  var response = await _authenticationService.AuthorizeLogin(loginVM);
            var handler = new HttpClientHandler();
            handler.UseCookies = true;
            handler.CookieContainer = new CookieContainer();

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:7186/api/");

            var loginDTO = new LoginDto
            {
                Login = "admin",
                Password = "admin"
            };
            var response = await client.PostAsJsonAsync($"Auth/Login", loginDTO);
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login));
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<ActionResult> RegisterAuthentication(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(nameof(Register), registerVM);
            
            if (await _userServices.GetUserByEmailAsync(registerVM.email) != null)
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
            _emailService.SendEmailAsync(registerVM.email, "Registration OTP",EmailTemplate.Registration, model);

            return View(nameof(OTPAuthentication));
        }


        [HttpPost]
        public async Task<ActionResult> OTPAuthentication(string OTP)
        {
            if (!ModelState.IsValid) return View(nameof(OTPAuthentication));
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
