using DiagramManager.Application.Interfaces;
using DiagramManager.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DiagramManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IDiagramService, DiagramService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
