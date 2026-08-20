using DiagramManager.Domain.Common;
using DiagramManager.Domain.Enums;

namespace DiagramManager.Domain.Entities;

public class DiagramShare : BaseEntity
{
    public Guid DiagramId { get; set; }
    public Guid SharedWithUserId { get; set; }
    public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.View;

    public Diagram? Diagram { get; set; }
}
