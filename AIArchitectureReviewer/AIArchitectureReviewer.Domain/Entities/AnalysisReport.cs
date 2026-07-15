using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class AnalysisReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DiagramVersionId { get; set; }
        public string DiagramType { get; set; } = string.Empty;
        public string RawAiResponse { get; set; } = string.Empty;
        public string MarkdownReport { get; set; } = string.Empty;
        public float TotalScore { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
