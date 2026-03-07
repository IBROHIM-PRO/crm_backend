using System.Reflection;
using CRMBanks.Core.Common;
using CRMBanks.Core.Services;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.Infrastructure;
using CRMBanks.Infrastructure.Data;
using CRMBanks.Infrastructure.Services;
using CRMBanks.SharedKernel.Common.Interfaces;
using CRMBanks.Web.Helper;
using CRMBanks.Web.Hubs;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Web.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString();
        
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<ICreditService, CreditService>();
        services.AddScoped<IDepositService, DepositService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<ISelectListService, SelectListService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<INotificationSender, SignalRNotificationSender>();

        return services;
    }
}
