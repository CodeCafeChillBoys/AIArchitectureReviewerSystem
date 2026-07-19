using System;
using System.Collections.Generic;

namespace DiagramManager.Domain.Entities
{
    public class Workspace
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Diagram> Diagrams { get; set; } = new List<Diagram>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
