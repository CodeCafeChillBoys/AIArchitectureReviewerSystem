namespace DiagramManager.Application.DTOs.Events;

public class DiagramProcessingRequestEvent
{
    public Guid DiagramId { get; set; }
    public Guid DiagramVersionId { get; set; }
    public string DiagramType { get; set; } = string.Empty;
    public string RawFormat { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string? ContentText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
