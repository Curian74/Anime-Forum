using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Common.EmailTemplate;
using Microsoft.AspNetCore.Mvc;
namespace Application.Interfaces.Email
{
    public interface IEmailService
    {
         Task SendEmailAsync(string toEmail, string subject, EmailTemplateEnum template, Dictionary<string, string> model);
    }
}
