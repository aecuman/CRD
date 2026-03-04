using CRD.Application.Common;
using CRD.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRD.Application.Users.Commands
{
    public class RegisterUserCommand:IRequest<(bool, string[])>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Designation { get; set; }
        public string DutyStation { get; set; }
        public string[] Roles { get; set; }
    }
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (bool, string[])>
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(IUserManager userManager, ILogger<RegisterUserCommandHandler> logger, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<(bool, string[])> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = command.Email,
                Email = command.Email,
                Firstname = command.Firstname,
                Lastname = command.Lastname,
                Title = command.Title,
                Designation = command.Designation,
                DutyStation = command.DutyStation,
                Role = command.Roles[0]
            };

            string tempPassword = GenerateTempPassword();
            var result = await _userManager.CreateUserAsync(applicationUser,command.Roles, tempPassword);
            Console.WriteLine("Password: " + tempPassword);

            if (result.Succeeded)
            {
             

                
                _logger.LogInformation("User {Email} registered successfully.", command.Email);
            }
            else
            {
                _logger.LogError("User registration failed for {Email}: {Errors}", command.Email, result.Errors);
            }
            // Log login event
            //await _auditLogger.LogActionAsync(applicationUser.Id, "Register", "User registered successfully. A password reset link has been sent to their email.");
            return result;
        }
        private string GenerateTempPassword()
        {
            return "Temp@" + Guid.NewGuid().ToString("N").Substring(6); // Example: Temp@A12B34
        }
    }
}
