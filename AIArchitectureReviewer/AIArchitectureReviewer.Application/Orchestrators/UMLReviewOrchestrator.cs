using System.Text.Json.Nodes;
using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Application.Interfaces.RAG;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using AIArchitectureReviewer.Application.Prompts;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator : IUMLReviewOrchestrator
    {
        private readonly IRAGService _ragService;
        private readonly IUnitOfWork _unitOfWork;

        public UMLReviewOrchestrator(IRAGService ragService, IUnitOfWork unitOfWork)
        {
            _ragService = ragService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewSessionResult> ProcessAsync(Guid versionId, byte[] imageBytes, string mimeType, string? customPrompt = null)
        {
            // 1. Vision Parser
            var (cleanJson, parsedDiagram) = await ExecuteVisionParserAsync(imageBytes, mimeType);

            // Use the real VersionId passed from DiagramManagerService
            var diagramVersionId = versionId;

            // Compute Report ID 
            var reportId = diagramVersionId;

            // Use ReportId as SessionId so chat history persists along with the architecture
            var sessionId = reportId.ToString();

            // Fetch RAG Context based on the parsed diagram or a generic query
            var ragContext = await _ragService.AnswerQuestionAsync("Tìm kiếm các quy tắc (system rules) và báo cáo lỗi liên quan đến kiến trúc này.", 3);

            // 2. Architecture Review & Scoring (Combined Request)
            var reviewAndScoreJsonString = await ExecuteReviewAndScoreAsync(cleanJson, ragContext, customPrompt);
            var parsedReviewAndScore = SafeJsonParse(CleanJsonResponse(reviewAndScoreJsonString));

            string reviewText = parsedReviewAndScore?["ReviewDetails"]?.GetValue<string>() ?? "";
            if (string.IsNullOrWhiteSpace(reviewText) && parsedReviewAndScore?["raw_text"] != null)
            {
                reviewText = parsedReviewAndScore["raw_text"]?.GetValue<string>() ?? reviewAndScoreJsonString;
            }
            if (string.IsNullOrWhiteSpace(reviewText))
            {
                reviewText = reviewAndScoreJsonString;
            }

            // 3. Auto-Refactoring (Separate Request to avoid long output truncation)
            var refactorText = await ExecuteAutoRefactoringAsync(cleanJson, reviewText);

            // 4. Parse AI Score or fallback to programmatic score
            float totalScore = 10f;
            JsonNode? scoreJson = null;

            var aiScoreNode = parsedReviewAndScore?["Score"] ?? parsedReviewAndScore?["score"];
            if (aiScoreNode != null)
            {
                var totalScoreNode = aiScoreNode["total_score"] ?? aiScoreNode["TotalScore"] ?? aiScoreNode["score"] ?? aiScoreNode["totalScore"];
                if (totalScoreNode != null && float.TryParse(totalScoreNode.ToString(), out float parsedScore))
                {
                    totalScore = parsedScore;
                    scoreJson = aiScoreNode;
                }
            }

            if (scoreJson == null)
            {
                var (calculatedScore, scoreDetails, calculatedScoreJson) = CalculateScoreProgrammatically(parsedDiagram);
                totalScore = calculatedScore;
                scoreJson = calculatedScoreJson;
            }

            var diagramType = parsedDiagram?["diagram_type"]?.ToString();

            if (string.IsNullOrWhiteSpace(diagramType))
            {
                throw new InvalidOperationException("Diagram type was not returned by Vision Parser.");
            }
            // Write Cache
            var newReport = new AIArchitectureReviewer.Domain.Entities.AnalysisReport
            {
                Id = reportId,
                DiagramVersionId = diagramVersionId,
                DiagramType = diagramType,
                RawAiResponse = reviewText,
                MarkdownReport = refactorText,
                TotalScore = totalScore,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AnalysisReports.AddAsync(newReport);
            await _unitOfWork.CompleteAsync();

            return new ReviewSessionResult
            {
                SessionId = sessionId,
                Diagram = parsedDiagram,
                Review = SafeJsonParse(JsonSerializerSafe(new { ReviewDetails = reviewText, RefactoredMermaid = refactorText })),
                Score = scoreJson
            };
        }

        public async Task<ReviewHistoryDetailDto?> GetReportByVersionIdAsync(Guid versionId)
        {
            // Tìm AnalysisReport có DiagramVersionId khớp
            var reports = await _unitOfWork.AnalysisReports.FindAsync(r => r.DiagramVersionId == versionId);
            var report = reports.FirstOrDefault();

            if (report == null) return null;

            return new ReviewHistoryDetailDto
            {
                Id = report.Id,
                TotalScore = report.TotalScore,
                CreatedAt = report.CreatedAt,
                Review = SafeJsonParse(JsonSerializerSafe(new { ReviewDetails = report.RawAiResponse, RefactoredMermaid = report.MarkdownReport }))
            };
        }
    }
}
