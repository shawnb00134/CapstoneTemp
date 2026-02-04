using System.Diagnostics;
using CAMCMSServer.Database.Context;
using CAMCMSServer.Database.Mappers;
using CAMCMSServer.Database.Repository;
using CAMCMSServer.Database.Service;
using CAMCMSServer.Utils;
using CAMCMSServer.Utils.Notification;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

const string devPolicyName = "dev";

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(devPolicyName,
        policy =>
        {
            policy.WithOrigins
                    ("https://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services to the container.
var services = builder.Services;

//services.AddCognitoIdentity();

TypeMapper.Initialize("webapi.Model");

TypeMapper.Initialize("CAMCMSServer.Model");

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

for (int i = 0; i <100; i++)
{
    Console.WriteLine("Environment: " + environment);
}


switch (environment)
{
    case "dev":
        services.AddSingleton<IDataContext, DevPostgreSqlDataContext>();
        services.AddSingleton<IFileService, DevFileService>();
        break;

    case "test":
        services.AddSingleton<IDataContext, TestPostgreSqlDataContext>();
        services.AddSingleton<IFileService, DevFileService>();
        break;

    default:
        services.AddSingleton<IDataContext, LocalPostgreSqlDataContext>();
        services.AddSingleton<IFileService, DevFileService>();
        break;
}


#region Scopes

services.AddScoped<IUserService, UserService>();
services.AddScoped<IUserRepository, UserRepository>();

services.AddScoped<IAuthenticationService, AuthenticationService>();

services.AddScoped<IElementRepository, ElementRepository>();
services.AddScoped<IElementService, ElementService>();

services.AddScoped<IElementSetService, ElementSetService>();
services.AddScoped<IElementSetRepository, ElementSetRepository>();

services.AddScoped<IModuleRepository, ModuleRepository>();
services.AddScoped<IModuleService, ModuleService>();

services.AddScoped<ILibraryFolderRepository, LibraryFolderRepository>();
services.AddScoped<ILibraryFolderService, LibraryFolderService>();

services.AddScoped<IPackageRepository, PackageRepository>();
services.AddScoped<IPackageService, PackageService>();

services.AddScoped<IPackageFolderModuleRepository, PackageFolderModuleRepository>();
services.AddScoped<IPackageFolderModuleService, PackageFolderModuleService>();

services.AddScoped<IPublishedModuleRepository, PublishedModuleRepository>();
services.AddScoped<IPublishedModuleService, PublishedModuleService>();

services.AddScoped<IOrganizationRepository, OrganizationRepository>();
services.AddScoped<IOrganizationService, OrganizationService>();

services.AddScoped<IOrganizationPackageRepository, OrganizationPackageRepository>();
services.AddScoped<IOrganizationPackageService, OrganizationPackageService>();

services.AddScoped<IStudioAsideService, StudioAsideService>();

services.AddScoped<IContentRoleRepository, ContentRoleRepository>();
services.AddScoped<IContentRoleService, ContentRoleService>();

services.AddScoped<IOrganizationContentRoleRepository, OrganizationContentRoleRepository>();
services.AddScoped<IOrganizationContentRoleService, OrganizationContentRoleService>();

services.AddScoped<IInvitationRepository, InvitationRepository>();


#endregion

services.AddScoped<INotificationService, NotificationService>();

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<IDataContext>();
    context.Init();
}
DefaultTypeMap.MatchNamesWithUnderscores = true;
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
TokenValidator.Configuration = app.Configuration;

app.UseCors(devPolicyName);

app.Run();