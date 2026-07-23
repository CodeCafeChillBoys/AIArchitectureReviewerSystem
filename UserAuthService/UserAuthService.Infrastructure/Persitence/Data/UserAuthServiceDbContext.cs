using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Domain.Entities;

namespace UserAuthService.Infrastructure.Persitence.Data;

public partial class UserAuthServiceDbContext : DbContext
{
    public UserAuthServiceDbContext()
    {
    }

    public UserAuthServiceDbContext(DbContextOptions<UserAuthServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<NotificationLog> NotificationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationLog>().ToTable("NOTIFICATION_LOGS");
        modelBuilder.Entity<NotificationLog>()
            .HasIndex(log => log.CreatedAt);
        modelBuilder.Entity<NotificationLog>()
            .HasIndex(log => new { log.Channel, log.Status });
        modelBuilder.Entity<NotificationLog>()
            .HasIndex(log => log.CorrelationId);
    }
}
