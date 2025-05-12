using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string from, string subject, string body,string? cc);
        Task SendNewUserEmail(string to, string userName, string tempPassword, string loginUrl);
        Task SendPasswordResetEmailAsync(string to, string userName, string resetUrl);
        Task SendPasswordUpdatedEmailAsync(string to, string username, string resetUrl);
    }
}
