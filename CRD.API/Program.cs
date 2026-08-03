using CRD.Domain.Identity;
using CRD.Infrastructure;
using CRD.Application;
using CRD.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using CRD.API.Services;
using CRD.Application.Common;
using CRD.Infrastructure.Identity;
using CRD.Persistence.Mongo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));*/

 
// add in services section
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddPersistence(builder.Configuration);
 builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IPublishedRateMongoService, PublishedRateMongoService>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName); // Uses full namespace to avoid conflicts
});

var corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
                      policy =>
                      {
                          policy.WithOrigins(corsOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

var app = builder.Build();

// ✅ Apply migrations and create database on startup
var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

// Retry logic to wait for SQL Server to be ready
int retryCount = 0;
int maxRetries = 120; // Increased from 30 to 120 (4 minutes)
bool dbReady = false;

while (!dbReady && retryCount < maxRetries)
{
    try
    {
        Console.WriteLine($"[{retryCount + 1}/{maxRetries}] Attempting to connect to database...");
        
        // Check if database exists, if not create and migrate
        if (!(await dbContext.Database.CanConnectAsync()))
        {
            throw new Exception("Cannot connect to database");
        }
        
        // Force database deletion and recreation if migration history is corrupted
        try
        {
            var migrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (migrations.Any())
            {
                Console.WriteLine($"⏳ Applying {migrations.Count()} pending migrations...");
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("✅ Migrations completed successfully!");
                dbReady = true;
            }
            else
            {
                // Check that the singular table name 'Workflow' exists (not the plural 'Workflows' from EnsureCreated)
                var conn = dbContext.Database.GetDbConnection();
                await conn.OpenAsync();
                bool singularExists = false;
                bool pluralExists = false;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('Workflow', 'Workflows')";
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var name = reader.GetString(0);
                        if (name == "Workflow") singularExists = true;
                        if (name == "Workflows") pluralExists = true;
                    }
                }

                if (!singularExists)
                {
                    Console.WriteLine($"⚠️ Table mismatch detected (singular 'Workflow' missing, plural exists: {pluralExists}). Applying migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("✅ Database updated with correct table names!");
                    dbReady = true;
                }
                else
                {
                    Console.WriteLine("✅ Database is up to date with correct table names!");
                    dbReady = true;
                }
            }
        }
        catch (Exception migrationEx) when (migrationEx.Message.Contains("pending") || migrationEx.Message.Contains("Pending"))
        {
            Console.WriteLine("⚠️ Pending model changes detected. Applying migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("✅ Database updated successfully!");
            dbReady = true;
        }
    }
    catch (Exception ex) when (ex.Message.Contains("network") || ex.Message.Contains("timeout") || ex.Message.Contains("accessible") || ex.Message.Contains("refused") || ex.Message.Contains("provider") || ex.Message.Contains("broken") || ex.Message.Contains("connect"))
    {
        // SQL Server not ready yet, retry
        retryCount++;
        if (retryCount < maxRetries)
        {
            Console.WriteLine($"⏳ Database not ready. Error: {ex.Message.Substring(0, Math.Min(80, ex.Message.Length))}... Retrying in 2s");
            await Task.Delay(2000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database error: {ex.Message}");
        Console.WriteLine($"Exception type: {ex.GetType().Name}");
        dbReady = true; // Stop retrying on non-network errors
    }
}

if (!dbReady)
{
    Console.WriteLine("⚠️ Database initialization timed out, continuing anyway...");
}

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    await SeedData(app);
    return;
}
if (args.Length == 1 && args[0].ToLower() == "seedworkflow")
{
    await SeedWorkFlow(app);
    return;
}
if (args.Length == 1 && args[0].ToLower() == "seedplants")
{
    await SeedPlants(app);
    return;
}
if (args.Length == 1 && args[0].ToLower() == "seedstructuretypes")
{
    await SeedStructureTypes(app);
    return;
}
if (args.Length >= 1 && args[0].ToLower() == "seedusers")
{
    await SeedUsers(app, args.Length > 1 ? args[1] : null);
    return;
}
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
//app.MapIdentityApi<ApplicationUser>();


app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();


async Task SeedData(IHost app)
{

    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedUsers>();
        await service.SeedDefaultUserAsync();
        
    }
}
async Task SeedWorkFlow(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<WorkflowSeeder>();
        await service.SeedDefaultWorkflowAsync();

    }
}

async Task SeedPlants(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        var service = scope.ServiceProvider.GetService<PlantSeeder>();
        
        // Paths to sample data files
        var languagesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "sample", "languages.xlsx");
        var cropsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "sample", "CropDataCSV.xlsx");
        var treesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "sample", "TreeDataCSVTest.xlsx");
        
        Console.WriteLine($"Seeding plants from:");
        Console.WriteLine($"  Languages: {languagesPath}");
        Console.WriteLine($"  Crops: {cropsPath}");
        Console.WriteLine($"  Trees: {treesPath}");
        
        await service.SeedPlantsFromExcelAsync(dbContext, languagesPath, cropsPath, treesPath);
        Console.WriteLine("Plant seeding completed.");
    }
}

async Task SeedUsers(IHost app, string csvFilePath)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedUsers>();
        
        // Default path if not provided
        if (string.IsNullOrEmpty(csvFilePath))
            csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "users_migration.csv");
        
        Console.WriteLine($"Seeding users from: {csvFilePath}");
        await service.SeedUsersFromCsvAsync(csvFilePath);
        Console.WriteLine("User seeding completed.");
    }
}

async Task SeedStructureTypes(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<StructureTypeSeeder>();
        await service.SeedDefaultStructureTypesAsync();
        Console.WriteLine("Structure Types seeding completed.");
    }
}

