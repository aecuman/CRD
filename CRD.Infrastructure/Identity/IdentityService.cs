using CRD.Application.Common;
using CRD.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CRD.Infrastructure.Identity
{
    public class IdentityService : IUserManager
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        /*private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;*/

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IEmailSender emailSender, IConfiguration configuration)
        {

            //  _context.cu = httpAccessor.HttpContext?.User.FindFirst(OpenIdConnectConstants.Claims.Subject)?.Value?.Trim();
            //  _context.CurrentUserId = httpAccessor.HttpContext?.User.FindFirst(ClaimConstants.Subject)?.Value?.Trim();
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _configuration = configuration;
            //_emailService = emailService;
            //_tokenService = tokenService;
            //_context = context;
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (!_userManager.SupportsUserLockout)
                    await _userManager.AccessFailedAsync(user);

                return false;
            }
            return true;
        }

        public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role)
        {
           /* if (claims == null)
                claims = new string[] { };

            string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
            if (invalidClaims.Any())
                return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
           */

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            /*
            role = await _roleManager.FindByNameAsync(role.Name);

            foreach (string claim in claims.Distinct())
            {
                result = await this._roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));

                if (!result.Succeeded)
                {
                    await DeleteRoleAsync(role);
                    return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }
            */
            return (true, new string[] { });
        }

        public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user,IEnumerable<string> roles, string password)
        {
            // var pwd = GeneratePassword();
            var newUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = user.Role,
                LockoutEnabled = user.LockoutEnabled,
               
            };
            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            user = await _userManager.FindByNameAsync(user.UserName);

            try
            {
                if (roles!= null && roles.Count() > 0)
                {
                    foreach (var role in roles)
                    {
                        if (!await _roleManager.RoleExistsAsync(role))
                            await _roleManager.CreateAsync(new ApplicationRole() { Name = role });
                        await _userManager.AddToRoleAsync(newUser, role);
                    }
                }

              await  _emailSender.SendNewUserEmail(user.Email, user.Fullname, password, _configuration["FrontendUrl"]);
                // result = await this._userManager.AddToRolesAsync(user, roles.Distinct());

                //await _userManager.AddPasswordAsync(user, pwd);
                //var email = "<p>Email: " + user.Email + "</p>" + "<p>Password: " + password + " </ p > ";
                // _emailService.Send(user.Email, "DBA Account", email);

                // await _userManager.SetTwoFactorEnabledAsync(user,true);
                //await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, new string[] { });
        }

        public Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
                return await DeleteUserAsync(user);

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<ApplicationRole> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName)
        {
            var role = await _roleManager.Roles
                          //.Include(r => r.Claims)
                          //.Include(r => r.Users)
                          .Where(r => r.Name == roleName)
                          .SingleOrDefaultAsync();

            return role;
        }

        public async Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<ApplicationRole> rolesQuery = _roleManager.Roles
                          // .Include(r => r.Claims)
                          // .Include(r => r.Users)
                           .OrderBy(r => r.Name);

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles;
        }

       /* public async Task<(ApplicationUser User, string[] Roles)?> GetUserAndRolesAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return null;

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();

            return (user, roles);
            return new NotImplementedException()
        }*/

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u=>u.Id==userId);
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        /**/public async Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize)
        {
            IQueryable<ApplicationUser> usersQuery = _userManager.Users
                           .Include(u => u.Roles)
                           .OrderBy(u => u.UserName);

            if (page != -1)
                usersQuery = usersQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await _roleManager.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();
            /* return users
                 .Select(u => (u, roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                 .ToList();*/

            return users.Select(u => (u,_userManager.GetRolesAsync(u).Result.ToArray())).ToList();
        }

        public Task<List<(ApplicationUser User, string[] Roles)>> GetUsersAndRolesByUserTypeAsync(int page, int pageSize, string userTypeId)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string newPassword)
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }
        public async Task<IdentityResult> ResetPasswordAsync(string email,string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            Console.WriteLine($"Old User PasswordHash: {user.PasswordHash}");

            var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token)); //Encoding.UTF8.GetString(Convert.FromBase64String(request.Token));

            // Console.WriteLine("verification: "+ decodedToken);
            Console.WriteLine($"New Password reset result: {newPassword}");
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
            if (result.Succeeded)
            {
                var updatedUser = await _userManager.FindByEmailAsync(email);
                Console.WriteLine($"Updated User PasswordHash: {updatedUser.PasswordHash}");
                               // var newtoken = await _userManager.GeneratePasswordResetTokenAsync(user);
                               // var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

                                //   var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={newtoken}&email={email}";
                                await _emailSender.SendPasswordUpdatedEmailAsync(user.Email, user.Fullname, "");
                Console.WriteLine($"Password reset result: {result.Succeeded}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }


                user.LastPasswordChangedAt = DateTime.Now;
            
            await _userManager.UpdateAsync(user);
            }
           
            return result;
        }
        public async Task<(bool, string)> CheckIfPasswordIsTemporaryAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            /* bool isTempPassword = await _userManager.CheckPasswordAsync(user, password); // Check if using temp password
             if (isTempPassword)
             {
                 var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                 var encodedToken = WebUtility.UrlEncode(token);
                 return (isTempPassword,encodedToken);
             }*/
            if (user == null)
                return (false, string.Empty); // User not found

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                return (false, string.Empty); // Wrong password

            // Check if this is the first login (null or default DateTime)
            bool isFirstLogin = user.LastPasswordChangedAt == null || user.LastPasswordChangedAt == default;

            if (isFirstLogin)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);

                return (true, encodedToken); // Force password change
            }
            return (false, string.Empty); // Check temp password prefix


        }

        /*public async Task<bool> TestCanDeleteRoleAsync(string roleId)
        {
            return !await _context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }*/

        public Task<bool> TestCanDeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdatePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
        {
           /* if (claims != null)
            {
                string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
                if (invalidClaims.Any())
                    return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
            }
           */

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            /*
            if (claims != null)
            {
                var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == ClaimConstants.Permission);
                var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

                var claimsToRemove = roleClaimValues.Except(claims).ToArray();
                var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

                if (claimsToRemove.Any())
                {
                    foreach (string claim in claimsToRemove)
                    {
                        result = await _roleManager.RemoveClaimAsync(role, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (claimsToAdd.Any())
                {
                    foreach (string claim in claimsToAdd)
                    {
                        result = await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
                        if (!result.Succeeded)
                            return (false, result.Errors.Select(e => e.Description).ToArray());
                    }
                }
            }*/

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user)
        {
            return await UpdateUserAsync(user, null);
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = userRoles.Except(roles).ToArray();
                var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }
        public string GeneratePassword()
        {
            var options = _userManager.Options.Password;

            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }

     
        /*
        private string GenerateJWT(string username,IEnumerable<string> roles)
        {
            var claims = new List<Claim> {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, username),
    };
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(configuration["ApplicationSettings:JWT_Secret"])
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }*/
       /*  public string GenerateJwtToken(string username, List<string> roles)
         {
             var claims = new List<Claim>
         {
             new Claim(JwtRegisteredClaimNames.Sub, username),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(ClaimTypes.NameIdentifier, username)
         };

             roles.ForEach(role =>
             {
                 claims.Add(new Claim(ClaimTypes.Role, role));
             });

             var token = _tokenService.GenerateToken(claims);

             return new JwtSecurityTokenHandler().WriteToken(token);
         }*/
    }


}
