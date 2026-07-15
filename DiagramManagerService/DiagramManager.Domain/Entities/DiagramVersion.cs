using System;

namespace DiagramManager.Domain.Entities
{
    public class DiagramVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DiagramId { get; set; }
        public int VersionNumber { get; set; }
        public string StorageUrl { get; set; } = string.Empty;
        public string RawFormat { get; set; } = string.Empty;
        public string Status { get; set; } = "Uploaded";
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public float? AiScore { get; set; }
        public string? AiReview { get; set; }
        public Diagram? Diagram { get; set; }
    }
}
