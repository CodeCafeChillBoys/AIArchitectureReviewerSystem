using System;

namespace DiagramManager.Domain.Entities
{
    public class DiagramShare
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DiagramId { get; set; }
        public Guid SharedWithUserId { get; set; }
        public string PermissionLevel { get; set; } = "Read";

        public Diagram? Diagram { get; set; }
    }
}
