using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Responses;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        public async Task<IEnumerable<ReviewHistoryDto>> GetReviewHistoryAsync()
        {
            var reports = await _unitOfWork.AnalysisReports.GetAllAsync();
            return reports.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewHistoryDto
            {
                Id = r.DiagramVersionId != System.Guid.Empty ? r.DiagramVersionId : r.Id,
                CreatedAt = r.CreatedAt,
                TotalScore = r.TotalScore,
                DiagramType = r.DiagramType
            });
        }

        public async Task<ReviewHistoryDetailDto?> GetReviewDetailAsync(System.Guid id)
        {
            var reports = await _unitOfWork.AnalysisReports.FindAsync(r => r.DiagramVersionId == id || r.Id == id);
            var report = reports.FirstOrDefault();
            if (report == null) return null;

            return new ReviewHistoryDetailDto
            {
                Id = report.DiagramVersionId != System.Guid.Empty ? report.DiagramVersionId : report.Id,
                CreatedAt = report.CreatedAt,
                TotalScore = report.TotalScore,
                Review = SafeJsonParse(JsonSerializerSafe(new { ReviewDetails = report.RawAiResponse, RefactoredMermaid = report.MarkdownReport }))
            };
        }
    }
}
