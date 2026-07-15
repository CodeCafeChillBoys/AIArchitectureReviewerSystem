using Microsoft.EntityFrameworkCore;
using UserAuthService.Infrastructure.Persitence.Data;

namespace UserAuthService.API.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserAuthServiceDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
