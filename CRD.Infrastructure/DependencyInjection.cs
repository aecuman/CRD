using CRD.Application.Common;
using CRD.Infrastructure.Identity;
using CRD.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserManager, IdentityService>();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<ITokenService, TokenService>();
            services.Configure<TokenConfig>(configuration.GetSection("JWT"));
            return services;
        }
    }
}
