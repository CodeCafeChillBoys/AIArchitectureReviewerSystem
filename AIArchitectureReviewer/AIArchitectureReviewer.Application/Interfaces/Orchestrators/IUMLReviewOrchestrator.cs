using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;

namespace AIArchitectureReviewer.Application.Interfaces.Orchestrators
{
    public interface IUMLReviewOrchestrator
    {
        Task<ReviewSessionResult> ProcessAsync(Guid versionId, byte[] imageBytes, string mimeType, string? customPrompt = null);
        Task<JsonNode?> CheckConsistencyAsync(List<JsonNode> diagrams);
        Task<IEnumerable<ReviewHistoryDto>> GetReviewHistoryAsync();
        Task<ReviewHistoryDetailDto?> GetReviewDetailAsync(Guid id);
        Task<ReviewHistoryDetailDto?> GetReportByVersionIdAsync(Guid versionId);
    }
}
