using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class ChatSession
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
