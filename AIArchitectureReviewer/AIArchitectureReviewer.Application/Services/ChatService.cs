using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.RAG;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Interfaces.Services;
using AIArchitectureReviewer.Domain.Entities;

namespace AIArchitectureReviewer.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRAGService _ragService;

        public ChatService(IUnitOfWork unitOfWork, IRAGService ragService)
        {
            _unitOfWork = unitOfWork;
            _ragService = ragService;
        }

        public async Task<ChatResponseDto> SendMessageAsync(Guid sessionId, ChatRequestDto request)
        {
            // 1. Get or create session
            var session = await _unitOfWork.ChatSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                session = new ChatSession { Id = sessionId };
                await _unitOfWork.ChatSessions.AddAsync(session);
                // Also save now so it can be attached
                await _unitOfWork.CompleteAsync();
            }

            // 2. Fetch history
            var history = await _unitOfWork.ChatMessages.FindAsync(m => m.ChatSessionId == sessionId);
            var sortedHistory = history.OrderBy(m => m.CreatedAt).ToList();

            // 3. Save user message
            var userMsg = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatSessionId = sessionId,
                Role = "user",
                Content = request.Message
            };
            await _unitOfWork.ChatMessages.AddAsync(userMsg);

            // 4. Call RAG with history and optionally Analysis Report context
            string systemPrompt = "Bạn là một trợ lý AI phân tích kiến trúc phần mềm chuyên nghiệp. Hãy trả lời các câu hỏi dựa trên lịch sử cuộc trò chuyện và thông tin ngữ cảnh được cung cấp.";
            
            var report = await _unitOfWork.AnalysisReports.GetByIdAsync(sessionId);
            if (report != null)
            {
                systemPrompt += $"\n\nDưới đây là thông tin về Báo cáo đánh giá kiến trúc hiện tại của hệ thống này:\n" +
                                $"- Điểm số kiến trúc: {report.TotalScore}/10.0\n" +
                                $"- Chi tiết đánh giá từ AI:\n{report.RawAiResponse}\n" +
                                $"- Sơ đồ Mermaid sau khi tối ưu:\n{report.MarkdownReport}\n";
            }
            
            var aiResponse = await _ragService.ChatWithHistoryAsync(systemPrompt, sortedHistory, request.Message);

            // 5. Save model response
            var modelMsg = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatSessionId = sessionId,
                Role = "model",
                Content = aiResponse
            };
            await _unitOfWork.ChatMessages.AddAsync(modelMsg);
            await _unitOfWork.CompleteAsync();

            return new ChatResponseDto
            {
                SessionId = sessionId,
                Message = aiResponse,
                CreatedAt = modelMsg.CreatedAt
            };
        }

        public async Task<System.Collections.Generic.IEnumerable<ChatMessage>> GetMessagesAsync(Guid sessionId)
        {
            var messages = await _unitOfWork.ChatMessages.FindAsync(m => m.ChatSessionId == sessionId);
            return messages.OrderBy(m => m.CreatedAt);
        }
    }
}
