using Microsoft.EntityFrameworkCore;
using UserAuthService.Domain.Entities;

namespace UserAuthService.Infrastructure.Persistence;

public class UserAuthDbContext : DbContext
{
    public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}
