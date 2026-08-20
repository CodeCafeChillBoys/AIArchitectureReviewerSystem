using System.Text.Json.Nodes;

namespace AIArchitectureReviewer.Application.DTOs.Responses
{
    public class ReviewSessionResult
    {
        public string SessionId { get; set; } = string.Empty;
        public JsonNode? Diagram { get; set; }
        public JsonNode? Review { get; set; }
        public JsonNode? Score { get; set; }
    }
}
