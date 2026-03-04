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
if (args.Length == 1 && args[0].ToLower() == "seeddata")
    await SeedData(app);
if (args.Length == 1 && args[0].ToLower() == "seedworkflow")
    await SeedWorkFlow(app);
if (args.Length >= 1 && args[0].ToLower() == "seedusers")
    await SeedUsers(app, args.Length > 1 ? args[1] : null);
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


app.MapControllers();
// add in middleware section before app.Run()
app.UseStaticFiles();
app.UseDefaultFiles();

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

