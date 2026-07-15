using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        public async Task<IEnumerable<ReviewHistoryDto>> GetReviewHistoryAsync()
        {
            var reports = await _unitOfWork.AnalysisReports.GetAllAsync();
            return reports.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewHistoryDto
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                TotalScore = r.TotalScore,
                DiagramType = r.DiagramType
            });
        }

        public async Task<ReviewHistoryDetailDto?> GetReviewDetailAsync(System.Guid id)
        {
            var report = await _unitOfWork.AnalysisReports.GetByIdAsync(id);
            if (report == null) return null;

            return new ReviewHistoryDetailDto
            {
                Id = report.Id,
                CreatedAt = report.CreatedAt,
                TotalScore = report.TotalScore,
                Review = SafeJsonParse(JsonSerializerSafe(new { ReviewDetails = report.RawAiResponse, RefactoredMermaid = report.MarkdownReport }))
            };
        }
    }
}
