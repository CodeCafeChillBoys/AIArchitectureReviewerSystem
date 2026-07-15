using System.Threading.Tasks;

namespace AIArchitectureReviewer.Application.Interfaces.RAG
{
    public interface IRAGService
    {
        Task<string> AnswerQuestionAsync(string question, int contextTopK = 5);
        Task<string> GenerateContentAsync(string systemPrompt, string userPrompt);
        Task<string> GenerateContentWithImageAsync(string systemPrompt, string userPrompt, byte[] imageBytes, string mimeType);
        Task<string> ChatWithHistoryAsync(string systemPrompt, System.Collections.Generic.IEnumerable<Domain.Entities.ChatMessage> history, string newQuestion);
    }
}
