using Microsoft.Extensions.DependencyInjection;
using UserAuthService.Application.Interfaces;
using UserAuthService.Application.Services;

namespace UserAuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
