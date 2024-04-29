using Helpdesk.Application.Services;
using Helpdesk.Domain.Repositories;
using Helpdesk.Infrastructure.Impl.Repositories;
using Helpdesk.Infrastructure.Impl.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpdesk.Infrastructure.Impl;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddSingleton<TelegramService>();
        services.AddSingleton<EmailButtonHandlerService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBotUserRepository, BotUserRepository>();
        services.AddScoped<IEmailService, EmailService>();
        return services;
    }
}