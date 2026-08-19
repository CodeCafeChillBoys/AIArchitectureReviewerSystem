namespace AIArchitectureReviewer.Application.DTOs.Requests
{
    public class CreatePromptTemplateDto
    {
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string DiagramType { get; set; } = null!;
    }
}
