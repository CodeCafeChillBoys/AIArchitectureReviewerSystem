using MassTransit;
using AIArchitectureReviewer.Application.Interfaces.AI;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Application.Services;
using AIArchitectureReviewer.Application.Interfaces.RAG;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Application.Orchestrators;
using AIArchitectureReviewer.Infrastructure.Embedding;
using AIArchitectureReviewer.Infrastructure.RAG;
using AIArchitectureReviewer.Infrastructure.Search;
using AIArchitectureReviewer.Infrastructure.Data;
using AIArchitectureReviewer.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Services
builder.Services.AddSingleton<IApiKeyProvider, AIArchitectureReviewer.Infrastructure.AI.RoundRobinApiKeyProvider>();
builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();
builder.Services.AddHttpClient<IRAGService, RAGService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AIArchitectureReviewer.API.Consumers.DiagramAnalysisConsumer>();
    x.AddConsumer<AIArchitectureReviewer.API.Consumers.DocumentConsistencyReviewRequestedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("diagram-analysis-queue", e =>
        {
            e.ConfigureConsumer<AIArchitectureReviewer.API.Consumers.DiagramAnalysisConsumer>(context);
        });
        cfg.ReceiveEndpoint("document-consistency-review-queue", e =>
        {
            e.ConfigureConsumer<AIArchitectureReviewer.API.Consumers.DocumentConsistencyReviewRequestedConsumer>(context);
        });
    });
});

builder.Services.AddScoped<IKeywordSearchService, KeywordSearchService>();
builder.Services.AddScoped<IVectorSearchService, VectorSearchService>();
builder.Services.AddScoped<IHybridSearchService, HybridSearchService>();
builder.Services.AddScoped<IUMLReviewOrchestrator, UMLReviewOrchestrator>();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseVector()));

// Repository & Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<ISystemRuleService, SystemRuleService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICodeExtractorService, AIArchitectureReviewer.Infrastructure.Services.CodeExtractorService>();
builder.Services.AddScoped<IPromptTemplateService, PromptTemplateService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Automatically create database and run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    AIArchitectureReviewer.Infrastructure.Data.DbInitializer.SeedPromptsAsync(db).GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/health", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    var healthy = await dbContext.Database.CanConnectAsync(cancellationToken);
    return Results.Json(
        new { Service = "AIArchitectureReviewer", Status = healthy ? "Healthy" : "Unhealthy", CheckedAtUtc = DateTime.UtcNow },
        statusCode: healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable);
});

app.MapControllers();


app.Run();
