using AIArchitectureReviewer.Application.Interfaces.AI;
using AIArchitectureReviewer.Application.Interfaces.RAG;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Infrastructure.AI;
using AIArchitectureReviewer.Infrastructure.Data;
using AIArchitectureReviewer.Infrastructure.Embedding;
using AIArchitectureReviewer.Infrastructure.Persistence.Repositories;
using AIArchitectureReviewer.Infrastructure.RAG;
using AIArchitectureReviewer.Infrastructure.Search;
using AIArchitectureReviewer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIArchitectureReviewer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));

        // Repository & Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // AI Services & Search
        services.AddSingleton<IApiKeyProvider, RoundRobinApiKeyProvider>();
        services.AddHttpClient<IEmbeddingService, EmbeddingService>();
        services.AddHttpClient<IRAGService, RAGService>();

        services.AddScoped<IKeywordSearchService, KeywordSearchService>();
        services.AddScoped<IVectorSearchService, VectorSearchService>();
        services.AddScoped<IHybridSearchService, HybridSearchService>();

        services.AddScoped<ICodeExtractorService, CodeExtractorService>();

        return services;
    }
}
