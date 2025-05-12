using CRD.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Users.Commands
{
    public class UpdatePasswordCommand:IRequest<(bool, string[])>
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, (bool, string[])>
    {
        private readonly IUserManager _userManager;

        public UpdatePasswordCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool, string[])> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserByIdAsync(request.UserId);
            if (user == null)
                return (false, ["User not found"]);
            var result = await _userManager.UpdatePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                user.LastPasswordChangedAt = DateTime.UtcNow;
              await  _userManager.UpdateUserAsync(user);
return result; 
            }
            return result;
            
        }
    }
}
