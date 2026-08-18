using DiagramManager.Domain.Common;

namespace DiagramManager.Domain.Entities;

public class DiagramShare : BaseEntity
{
    public Guid DiagramId { get; set; }
    public Guid SharedWithUserId { get; set; }
    public string PermissionLevel { get; set; } = string.Empty;

    public Diagram? Diagram { get; set; }
}
