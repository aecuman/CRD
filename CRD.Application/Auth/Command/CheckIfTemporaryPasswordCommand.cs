using CRD.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Auth.Command
{
    public  class CheckIfTemporaryPasswordCommand:IRequest<(bool,string)>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CheckIfTempoararyPasswordCommandHandler : IRequestHandler<CheckIfTemporaryPasswordCommand, (bool, string)>
    {
        private readonly IUserManager _userManager;

        public CheckIfTempoararyPasswordCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<(bool, string)> Handle(CheckIfTemporaryPasswordCommand request, CancellationToken cancellationToken)
        {
            return _userManager.CheckIfPasswordIsTemporaryAsync(request.Email,request.Password);
        }
    }
}
