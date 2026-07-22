using System.Text.Json.Nodes;
using AIArchitectureReviewer.Application.Prompts;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        public async Task<JsonNode?> ConformanceReviewAsync(Guid reportId, Stream codeStream, string fileName)
        {
            // 1. Retrieve the existing AnalysisReport by reportId
            var existingReport = await _unitOfWork.AnalysisReports.GetByIdAsync(reportId);
            if (existingReport == null)
            {
                throw new ArgumentException($"UML Analysis Report with ID {reportId} not found.");
            }

            // The UML diagram JSON string is stored in ParsedDiagram
            var umlJson = existingReport.ParsedDiagram;

            // 2. Extract and format the uploaded source code files
            var formattedCode = await _codeExtractorService.ExtractCodeAsync(codeStream, fileName);

            // 3. Invoke Gemini to compare the UML JSON structure with the actual source code
            var userPrompt = $"CẤU TRÚC UML THIẾT KẾ (JSON):\n{umlJson}\n\nMÃ NGUỒN THỰC TẾ:\n{formattedCode}";

            var prompt = await GetPromptContentAsync("Conformance Review", SystemPrompts.ConformanceReviewPrompt);
            var responseText = await _ragService.GenerateContentAsync(
                prompt,
                userPrompt);

            // 4. Clean JSON response and parse it
            var cleanJson = CleanJsonResponse(responseText);
            return SafeJsonParse(cleanJson);
        }
    }
}
