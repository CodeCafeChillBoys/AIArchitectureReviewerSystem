namespace AIArchitectureReviewer.Application.DTOs
{
    public class UpdatePromptTemplateDto
    {
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string DiagramType { get; set; } = null!;
    }
}
