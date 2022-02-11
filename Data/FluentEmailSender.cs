using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ReactProjectsAuthApi.Data
{
    public class FluentEmailSender : IEmailSender
    {
        public FluentEmailSender(IConfiguration config)
        {
            var sender = new SmtpSender(new SmtpClient("smtp.ethereal.email")
            {
                EnableSsl = true,
                Port = 587,
                Credentials = new NetworkCredential(config["Mailing:Email"], config["Mailing:Password"])
            });

            Email.DefaultSender = sender;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Email.From("marshall.lesch5@ethereal.email").To(email)
                .Subject("Email confirmation link").Body(htmlMessage, isHtml: true).SendAsync();
        }
    }
}
