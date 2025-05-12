using CRD.Application.Common;
using CRD.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager().AddDefaultTokenProviders();
            services.AddScoped<SignInManager<ApplicationUser>>();
            services.AddDataProtection();

            /*services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration.GetValue<string>("Jwt:JwtIssuer"),
            ValidAudience = configuration.GetValue<string>("Jwt:JwtIssuer"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:JwtKey"))),
            ClockSkew = TimeSpan.Zero // remove delay of token when expire
        };
    });*/

            services.AddTransient<SeedUsers>();
            services.AddTransient<WorkflowSeeder>();
            services.AddTransient(typeof(IRepository<>), typeof(EFRepository<>));
            //.AddDefaultTokenProviders();

            /* services.AddIdentityServer()
                 .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            services.AddAuthentication()
                .AddIdentityServerJwt();*/


            /* var serviceClientSettingsConfig = configuration.GetSection("RabbitMq");
             services.Configure<RabbitMqConfiguration>(serviceClientSettingsConfig);
             services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
             services.AddTransient<IEmailService, EmailService>();*/
            return services;
        }
    }
}
