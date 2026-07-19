using System.Text.Json.Nodes;
using AIArchitectureReviewer.Application.Prompts;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        private async Task<(string cleanJson, JsonNode? parsedDiagram)> ExecuteVisionParserAsync(byte[] fileBytes, string mimeType)
        {
            string visionResultText;
            if (mimeType.StartsWith("image/") || mimeType == "application/pdf")
            {
                visionResultText = await _ragService.GenerateContentWithImageAsync(
                    SystemPrompts.VisionParserPrompt,
                    "Vui lòng phân tích sơ đồ kiến trúc trong hình ảnh này và trả về định dạng JSON.",
                    fileBytes,
                    mimeType);
            }
            else
            {
                var textContent = System.Text.Encoding.UTF8.GetString(fileBytes);
                visionResultText = await _ragService.GenerateContentAsync(
                    SystemPrompts.VisionParserPrompt,
                    $"Vui lòng phân tích sơ đồ kiến trúc trong đoạn mã dưới đây và trả về định dạng JSON:\n\n{textContent}");
            }

            var cleanJson = CleanJsonResponse(visionResultText);
            var parsedDiagram = SafeJsonParse(cleanJson);
            return (cleanJson, parsedDiagram);
        }

        private async Task<string> ExecuteReviewAndScoreAsync(string jsonDiagram, string ragContext, string? customPrompt)
        {
            string systemPrompt = SystemPrompts.ReviewAndScorePrompt;
            if (!string.IsNullOrWhiteSpace(customPrompt))
            {
                systemPrompt += $"\n\nYÊU CẦU ĐẶC BIỆT TỪ NGƯỜI DÙNG: {customPrompt}";
            }

            return await _ragService.GenerateContentAsync(
                systemPrompt,
                $"Dưới đây là cấu trúc JSON của hệ thống:\n{jsonDiagram}\n\nKiến thức RAG (Quy tắc hệ thống):\n{ragContext}");
        }

        private async Task<string> ExecuteAutoRefactoringAsync(string jsonDiagram, string reviewText)
        {
            return await _ragService.GenerateContentAsync(
                SystemPrompts.AutoRefactoringPrompt,
                $"Cấu trúc JSON gốc:\n{jsonDiagram}\n\nCác lỗi kiến trúc:\n{reviewText}\n\nHãy sửa lỗi và xuất ra Mermaid code.");
        }

        public async Task<JsonNode?> CheckConsistencyAsync(List<JsonNode> diagrams)
        {
            var diagramsJson = string.Join("\n\n---\n\n", diagrams);
            var consistencyText = await _ragService.GenerateContentAsync(
                SystemPrompts.ConsistencyCheckPrompt,
                $"Vui lòng kiểm tra tính nhất quán của các sơ đồ sau:\n{diagramsJson}");

            return SafeJsonParse(JsonSerializerSafe(new { ConsistencyReport = consistencyText }));
        }
    }
}
