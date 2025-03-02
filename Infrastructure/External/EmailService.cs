﻿using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Application.Interfaces.Email;
using Application.Common.EmailTemplate;
using System.Collections.Generic;
using System.Text;
namespace Infrastructure.External
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly TemplateBody _templateBody;

        public EmailService(IConfiguration configuration, TemplateBody templateBody)
        {
            _configuration = configuration;
            _templateBody = templateBody;
        }
        public async Task SendEmailAsync(string toEmail, string subject, EmailTemplateEnum template, Dictionary<string, string> model)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string body = _templateBody.GetEmailBody(template, model);
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }  
    }
}
