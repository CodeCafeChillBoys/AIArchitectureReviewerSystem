using DiagramManager.Application.Interfaces;
using DiagramManager.Infrastructure.Data;
using DiagramManager.Infrastructure.Services;
using MassTransit;
using DiagramManager.Application.Services;
using DiagramManager.API.GrpcClients;
using Microsoft.EntityFrameworkCore;
using DiagramManager.API.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter an admin JWT token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// File Storage
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IDocumentExtractorService, DocumentExtractorService>();
builder.Services.AddScoped<IStorageMonitoringService, LocalStorageMonitoringService>();

// Database
builder.Services.AddDbContext<WorkspaceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MassTransit / RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DiagramManager.API.Consumers.DiagramAnalysisCompletedConsumer>();
    x.AddConsumer<DiagramManager.API.Consumers.DocumentConsistencyReviewCompletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("diagram-analysis-completed-queue", e =>
        {
            e.ConfigureConsumer<DiagramManager.API.Consumers.DiagramAnalysisCompletedConsumer>(context);
        });

        cfg.ReceiveEndpoint("document-consistency-completed-queue", e =>
        {
            e.ConfigureConsumer<DiagramManager.API.Consumers.DocumentConsistencyReviewCompletedConsumer>(context);
        });
    });
});

// gRPC Client for UserAuthService
builder.Services.AddGrpcClient<AuthGrpc.AuthGrpcClient>(o =>
{
    // Cấu hình URL của UserAuthService (HTTPS port 7158)
    o.Address = new Uri("https://localhost:7158");
});

builder.Services.AddAuthentication(GrpcAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, GrpcAuthenticationHandler>(
        GrpcAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("1");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkspaceDbContext>();
    // Make sure to add migrations first: dotnet ef migrations add Initial
    try { db.Database.Migrate(); } catch { }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", async (
    WorkspaceDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    var healthy = await dbContext.Database.CanConnectAsync(cancellationToken);
    return Results.Json(
        new
        {
            Service = "DiagramManagerService",
            Status = healthy ? "Healthy" : "Unhealthy",
            CheckedAtUtc = DateTime.UtcNow
        },
        statusCode: healthy
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable);
}).AllowAnonymous();

app.MapControllers();

app.Run();
