using DiagramManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiagramManager.Infrastructure.Data
{
    public class WorkspaceDbContext : DbContext
    {
        public WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : base(options)
        {
        }

        public DbSet<Workspace> Workspaces { get; set; } = null!;
        public DbSet<Diagram> Diagrams { get; set; } = null!;
        public DbSet<DiagramVersion> DiagramVersions { get; set; } = null!;
        public DbSet<DiagramShare> DiagramShares { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workspace>().ToTable("WORKSPACES");
            modelBuilder.Entity<Diagram>().ToTable("DIAGRAMS");
            modelBuilder.Entity<DiagramVersion>().ToTable("DIAGRAM_VERSIONS");
            modelBuilder.Entity<DiagramShare>().ToTable("DIAGRAM_SHARES");

            modelBuilder.Entity<Diagram>()
                .HasOne(d => d.Workspace)
                .WithMany(w => w.Diagrams)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiagramVersion>()
                .HasOne(dv => dv.Diagram)
                .WithMany(d => d.Versions)
                .HasForeignKey(dv => dv.DiagramId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiagramShare>()
                .HasOne(ds => ds.Diagram)
                .WithMany(d => d.Shares)
                .HasForeignKey(ds => ds.DiagramId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
