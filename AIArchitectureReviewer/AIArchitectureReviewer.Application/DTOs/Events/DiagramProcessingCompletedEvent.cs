using AIArchitectureReviewer.Domain.Enums;

namespace DiagramManager.Application.DTOs.Events;

public class DiagramProcessingCompletedEvent
{
    public Guid DiagramId { get; set; }
    public Guid DiagramVersionId { get; set; }
    public bool IsSuccess { get; set; }
    public DiagramVersionStatus Status { get; set; }
    public string? ReviewResultSummary { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
