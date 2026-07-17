using System;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class ConsistencyReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkspaceId { get; set; }
        public string DiagramNames { get; set; } = string.Empty;
        public string ReportData { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
