using System;

namespace AIArchitectureReviewer.Application.DTOs
{
    public class ChatResponseDto
    {
        public Guid SessionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
