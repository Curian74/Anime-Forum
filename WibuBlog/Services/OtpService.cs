using Application.Common.EmailTemplate;
using Application.Common.MessageOperations;
using Application.Interfaces.Email;
using Infrastructure.Extensions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using WibuBlog.ViewModels.Authentication;

namespace WibuBlog.Services
{
    public class OtpService(IEmailService emailService, IHttpContextAccessor httpContextAccessor)
    {
        private readonly IEmailService _emailService = emailService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task SendOtp(RegisterVM registerVM)
        {
            var otp = OTPGenerator.GenerateOTP();
            _httpContextAccessor.HttpContext?.Session.SetString("OTP", otp);
            _httpContextAccessor.HttpContext?.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(5).ToString());
            var registerData = JsonConvert.SerializeObject(registerVM);
            _httpContextAccessor.HttpContext?.Session.SetString("RegisterVM", registerData);
            var model = new Dictionary<string, string>
            {
                { "Name", registerVM.username },
                    { "OTP", otp  }
                };
            await _emailService.SendEmailAsync(registerVM.email, "Registration OTP", EmailTemplateEnum.Registration, model);
        }

        public async Task SendOtp(string username, string email)
        {
            var otp = OTPGenerator.GenerateOTP();
            _httpContextAccessor.HttpContext?.Session.SetString("OTP", otp);
            _httpContextAccessor.HttpContext?.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(5).ToString());
            var model = new Dictionary<string, string>
            {
                { "name", username},
                { "OTP", otp  }
                };
            await _emailService.SendEmailAsync(email, "Reset password OTP", EmailTemplateEnum.ForgotPassword, model);
        }

        public Dictionary<string, string> ValidateOTP(string savedOTP, string OTP, string expiryTimeStr)
        {
            Dictionary<string, string> errors = [];
            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(expiryTimeStr))
            {
                errors.Add("OTP", MessageConstants.MEO002);
                return errors;
            }
            if (!DateTime.TryParse(expiryTimeStr, out DateTime expiryTime))
            {
                errors.Add("OTP", MessageConstants.MEO001);
                return errors;
            }
            if (DateTime.UtcNow > expiryTime)
            {
                errors.Add("OTP", MessageConstants.MEN007);
                return errors;
            }
            if (!savedOTP.Equals(OTP))
            {
                errors.Add("OTP", MessageConstants.MEN006);
                return errors;
            }
            return errors;
        }
    }
}
