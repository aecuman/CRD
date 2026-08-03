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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

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

            /**/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = configuration["Jwt:JwtIssuer"],
        ValidAudience = configuration["Jwt:JwtAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:JwtKey"])),
        ClockSkew = TimeSpan.Zero // Ensure token expires exactly when it should
    };
});



            services.AddTransient<SeedUsers>();
            services.AddTransient<WorkflowSeeder>();
            services.AddTransient<PlantSeeder>();
            services.AddTransient<StructureTypeSeeder>();
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
