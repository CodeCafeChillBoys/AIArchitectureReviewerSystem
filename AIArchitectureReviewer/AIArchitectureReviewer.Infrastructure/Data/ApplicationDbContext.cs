using Microsoft.EntityFrameworkCore;
using AIArchitectureReviewer.Domain.Entities;

namespace AIArchitectureReviewer.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SystemRule> SystemRules { get; set; }
        public DbSet<RuleChunk> RuleChunks { get; set; }
        public DbSet<AnalysisReport> AnalysisReports { get; set; } = null!;
        public DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<RuleChunk>()
                .Property(c => c.Embedding)
                .HasColumnType("vector(768)");

            modelBuilder.Entity<RuleChunk>()
                .HasIndex(c => c.Embedding)
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");

            // Configure Table Names to match the ERD strictly (optional, but good for clarity)
            modelBuilder.Entity<SystemRule>().ToTable("SYSTEM_RULES");
            modelBuilder.Entity<AnalysisReport>().ToTable("ANALYSIS_REPORTS");
            modelBuilder.Entity<ChatSession>().ToTable("CHAT_SESSIONS");
            modelBuilder.Entity<ChatMessage>().ToTable("CHAT_MESSAGES");

            // ChatMessage -> ChatSession (Many-to-One)
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.ChatSession)
                .WithMany(s => s.Messages)
                .HasForeignKey(m => m.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
