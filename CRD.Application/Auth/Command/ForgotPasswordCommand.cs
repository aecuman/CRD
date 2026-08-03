using CRD.Application.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Auth.Command
{
    public class ForgotPasswordCommand:IRequest<bool>
    {
        public string Email { get; set; }
    }
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand,bool>
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(IAuthService authService, IEmailSender emailSender, IConfiguration configuration, ILogger<ForgotPasswordCommandHandler> logger)
        {
            _authService = authService;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ForgotPassword(request.Email);

            if (result.Item1)
            {
                var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={result.Item2}&email={request.Email}";
                // Fire-and-forget: return immediately, don't block on SMTP
                _ = Task.Run(async () =>
                {
                    try { await _emailSender.SendPasswordResetEmailAsync(request.Email, result.Item3, resetLink); }
                    catch (Exception ex) { _logger.LogError(ex, "Failed to send password reset email to {Email}", request.Email); }
                });
            }
            return result.Item1;
        }
    }
}
