using Application.Common.EmailTemplate;
namespace Application.Interfaces.Email
{
    public interface IEmailService
    {
         Task SendEmailAsync(string toEmail, string subject, EmailTemplateEnum template, Dictionary<string, string> model);
    }
}
