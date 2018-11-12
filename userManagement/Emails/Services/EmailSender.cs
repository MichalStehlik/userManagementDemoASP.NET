using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace userManagement.Services
{
    public class EmailSender : IEmailSender
    {
        public string HtmlMessage { get; set; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("////", "////"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("test-mail@pslib.cz")
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            if (HtmlMessage != "")
            {
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(HtmlMessage);
                htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                mailMessage.AlternateViews.Add(htmlView);
            }
            return client.SendMailAsync(mailMessage);
        }
    }
}
