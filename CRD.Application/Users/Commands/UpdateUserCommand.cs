using CRD.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Users.Commands
{
    public class UpdateUserCommand:IRequest<(bool, string[])>
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string[] Roles { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, (bool, string[])>
    {
        private readonly IUserManager _userManager;

        public UpdateUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool, string[])> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserByIdAsync(request.Id);
            if (user == null)
            {
                return (false, ["User does not exist"]);
            }
            user.Firstname = request.Firstname;
            user.Lastname = request.Lastname;
            user.Email = request.Email;
           user.Role = request.Role;
           // user.Roles = request.Roles;
           return await _userManager.UpdateUserAsync(user,request.Roles);

        }
    }
}
