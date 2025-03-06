using CRD.Application.Common;
using CRD.Domain.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Auth.Command
{
    public class UserLoginCommand:IRequest<LoginDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, LoginDto>
    {
        private IAuthService _authService;
        public readonly ILogger<UserLoginCommand> _logger;

        public UserLoginCommandHandler(IAuthService authService, ILogger<UserLoginCommand> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public Task<LoginDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("this is the login by user: {0}", request.Email);
            return _authService.LoginSystemUser(request.Email, request.Password);
        }
    }
}
