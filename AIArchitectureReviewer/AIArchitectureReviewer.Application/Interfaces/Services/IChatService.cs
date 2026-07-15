using System;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;

namespace AIArchitectureReviewer.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<ChatResponseDto> SendMessageAsync(Guid sessionId, ChatRequestDto request);
        Task<System.Collections.Generic.IEnumerable<AIArchitectureReviewer.Domain.Entities.ChatMessage>> GetMessagesAsync(Guid sessionId);
    }
}
