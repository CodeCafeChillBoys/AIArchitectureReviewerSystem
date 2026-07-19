using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramManager.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StorageUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public float? ConsistencyScore { get; set; }
        public string? ConsistencyReview { get; set; }
        public Workspace? Workspace { get; set; }
        public ICollection<Diagram> Diagrams { get; set; } = new List<Diagram>();
    }
}
