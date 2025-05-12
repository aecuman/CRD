using CRD.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common
{
    public interface IAuthService
    {
        Task<LoginDto> LoginSystemUser(string email, string password);
        Task<(bool, string,string)> ForgotPassword(string email);

    }
}
