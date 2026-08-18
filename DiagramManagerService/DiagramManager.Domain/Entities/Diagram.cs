using DiagramManager.Domain.Common;

namespace DiagramManager.Domain.Entities;

public class Diagram : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DiagramType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Workspace? Workspace { get; set; }
    public ICollection<DiagramVersion> DiagramVersions { get; set; } = new List<DiagramVersion>();
    public ICollection<DiagramShare> DiagramShares { get; set; } = new List<DiagramShare>();
}
