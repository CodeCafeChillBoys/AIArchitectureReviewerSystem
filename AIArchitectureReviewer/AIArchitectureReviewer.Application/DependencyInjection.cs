using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.Orchestrators;
using AIArchitectureReviewer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AIArchitectureReviewer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUMLReviewOrchestrator, UMLReviewOrchestrator>();
        services.AddScoped<ISystemRuleService, SystemRuleService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IPromptTemplateService, PromptTemplateService>();

        return services;
    }
}
