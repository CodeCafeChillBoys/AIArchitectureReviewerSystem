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
            
            // Try to extract content between ```json and ```
            var match = System.Text.RegularExpressions.Regex.Match(text, @"```(?:json)?\s*(.*?)\s*```", System.Text.RegularExpressions.RegexOptions.Singleline);
            if (match.Success)
            {
                text = match.Groups[1].Value.Trim();
            }
            
            // Fallback: manually find first '{' and last '}'
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

        private Guid ComputeHashBytes(byte[] inputBytes)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return new Guid(hashBytes);
            }
        }
    }
}
