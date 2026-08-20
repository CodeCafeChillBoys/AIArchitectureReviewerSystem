using System;
using System.Text.Json.Nodes;

namespace AIArchitectureReviewer.Application.DTOs.Responses
{
    public class ReviewHistoryDetailDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalScore { get; set; }
        public JsonNode? Review { get; set; }
    }
}
