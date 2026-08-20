using DiagramManager.Domain.Common;
using DiagramManager.Domain.Enums;

namespace DiagramManager.Domain.Entities;

public class DiagramVersion : BaseEntity
{
    public Guid DiagramId { get; set; }
    public int VersionNumber { get; set; }
    public string StorageUrl { get; set; } = string.Empty;
    public string RawFormat { get; set; } = string.Empty;
    public DiagramVersionStatus Status { get; set; } = DiagramVersionStatus.Pending;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Diagram? Diagram { get; set; }
}
