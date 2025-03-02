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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System.Net;
using WibuBlog.Services;
using WibuBlog.ViewModels.Authentication;
namespace WibuBlog.Controllers
{
    public class AuthenticationController(AuthenticationServices authenticationService,IOptions<AuthTokenOptions> authTokenOptions,
        IHttpClientFactory httpClientFactory, IEmailService emailService, UserServices userServices) : Controller
     {
        private readonly AuthenticationServices _authenticationService = authenticationService;
        private readonly AuthTokenOptions _authTokenOptions = authTokenOptions.Value;
        private readonly IEmailService _emailService = emailService;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly UserServices _userServices = userServices;
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
            if (await _userServices.GetUserByUsernameAsync(registerVM.username) != null)
            {
                ModelState.AddModelError("username", MessageConstants.MEN008);
                return View(nameof(Register), registerVM);
            }
            var model = _authenticationService.ProcessSessionData(registerVM);
            _ = _emailService.SendEmailAsync(registerVM.email, "Registration OTP", EmailTemplate.Registration, model);
            return View(nameof(OTPAuthentication));
        }


        [HttpPost]
        public async Task<ActionResult> OTPAuthentication(string OTP)
        {
            if (!ModelState.IsValid) return View(nameof(OTPAuthentication));
            var errors = await _authenticationService.AuthorizeRegister(OTP);
            if (errors.Count == 0)
            {
                TempData["RegistrationSuccess"] = MessageConstants.MEN003;
                return RedirectToAction(nameof(Login));
            }
            else
            {
                foreach (var x in errors)
                {
                    ModelState.AddModelError(x.Key, x.Value);
                }
                return View(nameof(OTPAuthentication));
            }
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View(NotFound());
        }
    }
}
