using CRD.Application.Common;
using CRD.Domain.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        public async Task SeedUsersFromCsvAsync(string csvFilePath)
        {
            logger.LogInformation("Starting bulk user seeding from CSV: {csvPath}", csvFilePath);

            if (!File.Exists(csvFilePath))
            {
                logger.LogError("CSV file not found: {csvPath}", csvFilePath);
                throw new FileNotFoundException($"CSV file not found: {csvFilePath}");
            }

            try
            {
                // Ensure Users role exists
                var usersRole = new ApplicationRole() { Name = "Users" };
                var roleResult = await userManager.CreateRoleAsync(usersRole);
                
                if (!roleResult.Succeeded && !roleResult.Errors.Any(e => e.Contains("already exists")))
                {
                    logger.LogWarning("Failed to create Users role. Continuing anyway.");
                }

                var users = ReadCsvFile(csvFilePath);
                int successCount = 0;
                int failureCount = 0;

                foreach (var userDto in users)
                {
                    try
                    {
                        // Check if user already exists
                        var existingUser = await userManager.GetUserByEmailAsync(userDto.Email);
                        if (existingUser != null)
                        {
                            logger.LogWarning("User {email} already exists. Skipping.", userDto.Email);
                            failureCount++;
                            continue;
                        }

                        var user = new ApplicationUser
                        {
                            UserName = userDto.UserName,
                            Email = userDto.Email,
                            Firstname = userDto.Firstname,
                            Lastname = userDto.Lastname,
                            Title = userDto.Title,
                            Role = userDto.Role,
                            Designation = userDto.Designation,
                            DutyStation = userDto.DutyStation,
                            EmailConfirmed = true,
                            LockoutEnabled = true
                        };

                        var createResult = await userManager.CreateUserAsync(user, new[] { userDto.Role }, userDto.Password);
                        
                        if (createResult.Succeeded)
                        {
                            successCount++;
                            logger.LogInformation("User {firstname} {lastname} ({email}) created successfully.", 
                                userDto.Firstname, userDto.Lastname, userDto.Email);
                        }
                        else
                        {
                            failureCount++;
                            logger.LogError("Failed to create user {email}. Errors: {errors}", 
                                userDto.Email, string.Join(", ", createResult.Errors));
                        }
                    }
                    catch (Exception ex)
                    {
                        failureCount++;
                        logger.LogError(ex, "Exception creating user {email}", userDto.Email);
                    }
                }

                logger.LogInformation("Bulk user seeding completed. Success: {success}, Failures: {failure}", 
                    successCount, failureCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during bulk user seeding");
                throw;
            }
        }

        private List<UserSeedDto> ReadCsvFile(string filePath)
        {
            var users = new List<UserSeedDto>();

            using (var reader = new StreamReader(filePath))
            {
                string line;
                bool isHeader = true;

                while ((line = reader.ReadLine()) != null)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue; // Skip header row
                    }

                    var fields = ParseCsvLine(line);
                    if (fields.Length < 9)
                    {
                        logger.LogWarning("Invalid CSV line (not enough fields): {line}", line);
                        continue;
                    }

                    users.Add(new UserSeedDto
                    {
                        Title = fields[0],
                        Firstname = fields[1],
                        Lastname = fields[2],
                        UserName = fields[3],
                        Email = fields[4],
                        Password = fields[5],
                        Role = fields[6],
                        Designation = fields[7],
                        DutyStation = fields[8]
                    });
                }
            }

            return users;
        }

        private string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString().Trim('"').Trim());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(line[i]);
                }
            }

            fields.Add(currentField.ToString().Trim('"').Trim());
            return fields.ToArray();
        }
    }
}
