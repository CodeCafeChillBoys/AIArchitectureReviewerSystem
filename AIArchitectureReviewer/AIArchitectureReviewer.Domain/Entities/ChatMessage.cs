using System;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid ChatSessionId { get; set; }
        
        // "user" or "model"
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship
        public ChatSession ChatSession { get; set; } = null!;
    }
}
