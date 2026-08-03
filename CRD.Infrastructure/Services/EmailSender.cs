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
using CRD.Infrastructure.Helpers;

namespace CRD.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _appSettings;

        public EmailSender(IOptions<MailSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task SendEmailAsync(string to, string from, string subject, string body, string? cc)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_appSettings.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            if (!string.IsNullOrEmpty(cc)) email.Cc.Add(MailboxAddress.Parse(cc));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_appSettings.Host, _appSettings.Port, SecureSocketOptions.StartTls, cts.Token);
            await smtp.AuthenticateAsync(_appSettings.Mail, _appSettings.Password, cts.Token);
            await smtp.SendAsync(email, cts.Token);
            await smtp.DisconnectAsync(true, cts.Token);
        }

        public async Task SendNewUserEmail(string to, string userName, string tempPassword, string loginUrl)
        {
            var body = EmailTemplateHelper.LoadTemplateFromEmbedded("NewUserTemplate.html", new Dictionary<string, string>
            {
                ["UserName"] = userName,
                ["UserEmail"] = to,
                ["TemporaryPassword"] = tempPassword,
                ["LoginUrl"] = loginUrl,
                ["AppUrl"]=_appSettings.AppUrl
            });
            await SendEmailAsync(to, "", "Your Compensation Rates DB Account is Ready", body,null);
           // await SendEmailAsync(to, "Your SRB Account is Ready", body, true);
        }

        public async Task SendPasswordResetEmailAsync(string to, string userName, string resetUrl)
        {
            var body = EmailTemplateHelper.LoadTemplateFromEmbedded("PasswordResetTemplate.html", new Dictionary<string, string>
            {
                ["UserName"] = userName,
                ["ResetUrl"] = resetUrl,
                ["AppUrl"] = _appSettings.AppUrl
            });
            await SendEmailAsync(to,"","Reset your Password",body,null);
           // await SendEmailAsync(to, "Reset Your SRB Password", body, true);
        }

        public async Task SendPasswordUpdatedEmailAsync(string to, string username, string resetUrl)
        {
            var body = EmailTemplateHelper.LoadTemplateFromEmbedded("PasswordUpdatedTemplate.html", new Dictionary<string, string>
            {
                ["UserName"] = username,
                ["ResetUrl"] = resetUrl,
                ["AppUrl"] = _appSettings.AppUrl
            });
            await SendEmailAsync(to, "", "Password Updated", body, null);
        }
    }

    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string AppUrl { get; set; }
    }
}
