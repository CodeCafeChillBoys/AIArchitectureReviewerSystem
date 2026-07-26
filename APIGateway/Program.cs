var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "UserAuthService API");
    c.SwaggerEndpoint("/diagram/swagger/v1/swagger.json", "DiagramManagerService API");
    c.SwaggerEndpoint("/ai/swagger/v1/swagger.json", "AIArchitectureReviewer API");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});

// app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();
