using System;

namespace AIArchitectureReviewer.Application.DTOs
{
    public class ReviewHistoryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public float TotalScore { get; set; }
        public string DiagramType { get; set; } = string.Empty;
    }
}
