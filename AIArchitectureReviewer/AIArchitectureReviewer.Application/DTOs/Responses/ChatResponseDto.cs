using System;

namespace AIArchitectureReviewer.Application.DTOs.Responses
{
    public class ChatResponseDto
    {
        public Guid SessionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
