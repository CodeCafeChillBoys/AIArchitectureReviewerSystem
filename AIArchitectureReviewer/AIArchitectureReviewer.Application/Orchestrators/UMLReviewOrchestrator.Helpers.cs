using System;
using System.Text.Json.Nodes;

namespace AIArchitectureReviewer.Application.Orchestrators
{
    public partial class UMLReviewOrchestrator
    {
        private string CleanJsonResponse(string aiResponse)
        {
            if (string.IsNullOrWhiteSpace(aiResponse)) return "{}";
            var text = aiResponse.Trim();

            // Find the outermost '{' and '}' to extract valid JSON
            int firstBrace = text.IndexOf('{');
            int lastBrace = text.LastIndexOf('}');
            if (firstBrace >= 0 && lastBrace >= firstBrace)
            {
                text = text.Substring(firstBrace, lastBrace - firstBrace + 1);
            }

            return text;
        }

        private JsonNode? SafeJsonParse(string jsonText)
        {
            try
            {
                return JsonNode.Parse(jsonText);
            }
            catch
            {
                // Fallback if AI didn't return valid JSON
                return JsonNode.Parse(JsonSerializerSafe(new { raw_text = jsonText }));
            }
        }

        private string JsonSerializerSafe(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
