using UserAuthService.API.Config;
using UserAuthService.Application;
using UserAuthService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// API Configurations (Cors & Swagger)
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();

// Clean Architecture layers
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAuthService API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI to load at root URL (http://localhost:5000/)
    });
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
