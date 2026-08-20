using DiagramManager.Infrastructure.Consumers;
using MassTransit;

namespace DiagramManager.API.Config
{
    public static class MassTransitConfig
    {
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<DiagramProcessingCompletedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration["RabbitMQ:Host"] ?? "localhost";
                    var username = configuration["RabbitMQ:Username"] ?? "guest";
                    var password = configuration["RabbitMQ:Password"] ?? "guest";
                    cfg.Host(host, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
