using System;
using System.Collections.Generic;

namespace DiagramManager.Domain.Entities
{
    public class Diagram
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Workspace? Workspace { get; set; }
        public ICollection<DiagramVersion> Versions { get; set; } = new List<DiagramVersion>();
        public ICollection<DiagramShare> Shares { get; set; } = new List<DiagramShare>();
    }
}
