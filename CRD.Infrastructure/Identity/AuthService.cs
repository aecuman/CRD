using CRD.Application.Common;
using CRD.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, ITokenService tokenService, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<LoginDto> LoginSystemUser(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var result = await _signInManager.PasswordSignInAsync(user?.UserName, password, false, true);
                var roles = await _userManager.GetRolesAsync(user);
                return new LoginDto(true, "Login has been successfull", _tokenService.GenerateToken(user.UserName, roles.ToList()), new UserDto() { Id = user.Id, FullName = user.Firstname + " " + user.Lastname, Email = user.Email, Roles = roles });

            }
            catch (Exception ex)
            {
                return new LoginDto(false, "Invalid username or password.", null, null);
            }

        }      
    }
}
