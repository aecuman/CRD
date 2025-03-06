using CRD.Application.Common;
using CRD.Domain.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Persistence
{
    public class SeedUsers
    {
        private readonly IUserManager userManager;
        private readonly ILogger<SeedUsers> logger;

        public SeedUsers(IUserManager userManager, ILogger<SeedUsers> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task SeedDefaultUserAsync()
        {
            logger.LogInformation("seeding is starting");
           var admin_role = new ApplicationRole() { Name = "admin" };
            
            var resultRole=await userManager.CreateRoleAsync(admin_role);

            if (resultRole.Succeeded)
            {
                var user = new ApplicationUser() {UserName="admin", Firstname = "John", Lastname = "Doe", Email = "admin@example.com",Role="admin" };
                var result = await userManager.CreateUserAsync(user, new[] { user.Role }, "@Pa12345678");
                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{user.Firstname}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }
        }
}
