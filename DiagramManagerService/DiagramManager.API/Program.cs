using DiagramManager.Application.Interfaces;
using DiagramManager.Infrastructure.Data;
using DiagramManager.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using DiagramManager.API.GrpcClients;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// File Storage
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Database
builder.Services.AddDbContext<WorkspaceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MassTransit / RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DiagramManager.API.Consumers.DiagramAnalysisCompletedConsumer>();

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
    });
});

// gRPC Client for UserAuthService
builder.Services.AddGrpcClient<AuthGrpc.AuthGrpcClient>(o =>
{
    // Cấu hình URL của UserAuthService (HTTPS port 7158)
    o.Address = new Uri("https://localhost:7158");
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

// Custom Auth Middleware via gRPC
app.Use(async (context, next) =>
{
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (!string.IsNullOrEmpty(token))
    {
        var grpcClient = context.RequestServices.GetRequiredService<AuthGrpc.AuthGrpcClient>();
        try
        {
            var response = await grpcClient.ValidateTokenAsync(new TokenRequest { Token = token });
            if (response.IsValid)
            {
                string claimType1 = System.Security.Claims.ClaimTypes.NameIdentifier;
                string claimValue1 = response.UserId ?? "";
                string claimType2 = System.Security.Claims.ClaimTypes.Email;
                string claimValue2 = response.Email ?? "";
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(claimType1, claimValue1),
                    new System.Security.Claims.Claim(claimType2, claimValue2)
                };
                var identity = new System.Security.Claims.ClaimsIdentity(claims, "gRPCAuth");
                context.User = new System.Security.Claims.ClaimsPrincipal(identity);
            }
        }
        catch { /* Invalid token or gRPC error */ }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
