using DiagramManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiagramManager.Infrastructure.Persistence;

public class DiagramDbContext : DbContext
{
    public DiagramDbContext(DbContextOptions<DiagramDbContext> options) : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Diagram> Diagrams => Set<Diagram>();
    public DbSet<DiagramVersion> DiagramVersions => Set<DiagramVersion>();
    public DbSet<DiagramShare> DiagramShares => Set<DiagramShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Workspace configuration
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.ToTable("WORKSPACES");
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Name).IsRequired().HasMaxLength(200);
            entity.Property(w => w.UserId).IsRequired();
            entity.Property(w => w.CreatedAt).HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });

        // Diagram configuration
        modelBuilder.Entity<Diagram>(entity =>
        {
            entity.ToTable("DIAGRAMS");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            entity.Property(d => d.DiagramType).IsRequired().HasMaxLength(50);
            entity.Property(d => d.CreatedAt).HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.HasOne(d => d.Workspace)
                .WithMany(w => w.Diagrams)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DiagramVersion configuration
        modelBuilder.Entity<DiagramVersion>(entity =>
        {
            entity.ToTable("DIAGRAM_VERSIONS");
            entity.HasKey(dv => dv.Id);
            entity.Property(dv => dv.StorageUrl).IsRequired().HasMaxLength(500);
            entity.Property(dv => dv.RawFormat).HasMaxLength(100);
            entity.Property(dv => dv.Status).HasMaxLength(50);
            entity.Property(dv => dv.UploadedAt).HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.HasOne(dv => dv.Diagram)
                .WithMany(d => d.DiagramVersions)
                .HasForeignKey(dv => dv.DiagramId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DiagramShare configuration
        modelBuilder.Entity<DiagramShare>(entity =>
        {
            entity.ToTable("DIAGRAM_SHARES");
            entity.HasKey(ds => ds.Id);
            entity.Property(ds => ds.SharedWithUserId).IsRequired();
            entity.Property(ds => ds.PermissionLevel).HasMaxLength(50);

            entity.HasOne(ds => ds.Diagram)
                .WithMany(d => d.DiagramShares)
                .HasForeignKey(ds => ds.DiagramId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
