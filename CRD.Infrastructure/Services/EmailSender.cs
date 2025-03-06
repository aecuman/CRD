using CRD.Application.Common;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _appSettings;

        public EmailSender(IOptions<MailSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public Task SendEmailAsync(string to, string from, string subject, string body, string? cc)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_appSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            if(!string.IsNullOrEmpty(cc))email.Cc.Add(MailboxAddress.Parse(cc));
           

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_appSettings.Host, _appSettings.Port, SecureSocketOptions.None);
            smtp.Authenticate(_appSettings.Mail, _appSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
           return Task.FromResult(0);
        }
    }

    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
