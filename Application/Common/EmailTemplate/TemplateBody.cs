﻿using System.Text;

namespace Application.Common.EmailTemplate
{
    public class TemplateBody
    {
        public string GetEmailBody(EmailTemplateEnum template, Dictionary<string,string> model)
        {
            string body = GenerateEmailBody(template);
            body = LoadEmailBody(body,model);
            return body;
        }
        private string GenerateEmailBody(EmailTemplateEnum template)
        {
            switch (template)
            {
                case EmailTemplateEnum.Registration:
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<html>");
                    sb.AppendLine("<head>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("body { font-family: Arial, sans-serif; }");
                    sb.AppendLine(".container { padding: 20px; background: #f3f3f3; }");
                    sb.AppendLine(".content { background: white; padding: 20px; border-radius: 5px; }");
                    sb.AppendLine("</style>");
                    sb.AppendLine("</head>");
                    sb.AppendLine("<body>");
                    sb.AppendLine("<div class='container'>");
                    sb.AppendLine("<div class='content'>");
                    sb.AppendLine("<h2>Hello</h2>");
                    sb.AppendLine("<p>We've received a registration request for <b>{Name}</b> </p>");
                    sb.AppendLine("<p>Please use this OTP to complete the process</p>");
                    sb.AppendLine("<p><strong>{OTP}</strong></p>");
                    sb.AppendLine("<p>Thanks & Regards!</p>");
                    sb.AppendLine("<p>WBB Team.</p>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</body>");
                    sb.AppendLine("</html>");
                    return sb.ToString();

                case EmailTemplateEnum.ForgotPassword:
                    StringBuilder sb2 = new StringBuilder();
                    sb2.AppendLine("<html>");
                    sb2.AppendLine("<head>");
                    sb2.AppendLine("<style>");
                    sb2.AppendLine("body { font-family: Arial, sans-serif; }");
                    sb2.AppendLine(".container { padding: 20px; background: #f3f3f3; }");
                    sb2.AppendLine(".content { background: white; padding: 20px; border-radius: 5px; }");
                    sb2.AppendLine("</style>");
                    sb2.AppendLine("</head>");
                    sb2.AppendLine("<body>");
                    sb2.AppendLine("<div class='container'>");
                    sb2.AppendLine("<div class='content'>");
                    sb2.AppendLine("<h2>Hello</h2>");
                    sb2.AppendLine("<p>We have just received a password reset request for <b>{name}</b> </p>");
                    sb2.AppendLine("<p>Please use this OTP to complete the process</p>");
                    sb2.AppendLine("<p><strong>{OTP}</strong></p>");
                    sb2.AppendLine("<p>Thanks & Regards!</p>");
                    sb2.AppendLine("<p>WBB Team.</p>");
                    sb2.AppendLine("</div>");
                    sb2.AppendLine("</div>");
                    sb2.AppendLine("</body>");
                    sb2.AppendLine("</html>");
                    return sb2.ToString();

                default:
                    break;
            }
            return "sumtingwong";
        }

        private string LoadEmailBody(string template, Dictionary<string, string> model)
        {
            foreach (var x in model)
            {
                template = template.Replace($"{{{x.Key}}}", x.Value);
            }
            return template;
        }


    }
}
