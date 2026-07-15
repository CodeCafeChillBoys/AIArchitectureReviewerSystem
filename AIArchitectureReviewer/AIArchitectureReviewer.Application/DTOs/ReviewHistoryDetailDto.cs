using System;
using System.Text.Json.Nodes;

namespace AIArchitectureReviewer.Application.DTOs
{
    public class ReviewHistoryDetailDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalScore { get; set; }
        public JsonNode? Review { get; set; }
    }
}
