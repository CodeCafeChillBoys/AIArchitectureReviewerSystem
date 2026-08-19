namespace DiagramManager.Application.DTOs.Request;

public class CreateWorkspaceRequestDto
{
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}