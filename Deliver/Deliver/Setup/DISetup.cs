using Integrations.Impl;
using Integrations.Interface;
using Models;
using Models.Integration;
using Repository.Repository.Impl;
using Repository.Repository.Interface;
using Services.Impl;
using Services.Impl.Utils;
using Services.Interface;
using Services.Interface.Utils;
using System.Net.Mail;

namespace Deliver.Setup;

public static class DISetup
{
    public static IServiceCollection RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ILocationReposiotry, LocationRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<IPasswordRecoveryRepository, PasswordRecoveryRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        return services;
    }

    public static IServiceCollection RegisterService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICarService, CarService>();
        return services;
    }

    public static IServiceCollection RegisterIntegrations(this IServiceCollection services)
    {
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<SmtpClient>();
        return services;
    }

    public static IServiceCollection RegisterUtils(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationUtils, AuthenticationUtils>();
        services.AddScoped<IRoleUtils, RoleUtils>();
        services.AddScoped<ICompanyUtils, CompanyUtils>();
        services.AddScoped<IUserUtils, UserUtils>();
        services.AddScoped<ILocationUtils, LocationUtils>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IDeliveryUtils, DeliveryUtils>();
        return services;
    }

    public static IServiceCollection RegisterSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"))
            .AddScoped<AppSettings>();

        services.Configure<MailSettings>(configuration.GetSection("Mail"))
            .AddScoped<MailSettings>();

        services.Configure<LoggedUser>(configuration.GetSection("AppUser"))
            .AddScoped<LoggedUser>();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
            .AddScoped<JwtSettings>();

        return services;
    }
}
