using DiagramManager.Domain.Common;

namespace DiagramManager.Domain.Entities;

public class Workspace : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Diagram> Diagrams { get; set; } = new List<Diagram>();
}
