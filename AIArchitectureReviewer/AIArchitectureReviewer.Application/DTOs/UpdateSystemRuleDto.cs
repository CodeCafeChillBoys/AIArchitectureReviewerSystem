namespace AIArchitectureReviewer.Application.DTOs
{
    public class UpdateSystemRuleDto
    {
        public string DiagramType { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string RegexOrCondition { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
