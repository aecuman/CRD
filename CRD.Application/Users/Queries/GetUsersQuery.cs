using CRD.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Users.Queries
{
    public class GetUsersQuery:IRequest<List<UserViewModel>>
    {
        public int Page {  get; set; }  
        public int PageSize { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime? LastPasswordChangedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsLocked { get; set; }
    }
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserViewModel>>
    {
        private readonly IUserManager _userManager;

        public GetUsersQueryHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserViewModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var list = await _userManager.GetUsersAndRolesAsync(request.Page,request.PageSize);
            return list.ToList().Select(u=>new UserViewModel()
            {
                FullName = u.User.Fullname,
                Email = u.User.Email,
                Role = u.User.Role,
                Id = u.User.Id,
                Firstname = u.User.Firstname,
                Lastname = u.User.Lastname,
                IsLocked = u.User.LockoutEnabled,
                LastLoginAt = u.User.LastLoginAt,
                LastPasswordChangedAt = u.User.LastPasswordChangedAt              
                
                
                
            }).ToList();
        }
    }
}
