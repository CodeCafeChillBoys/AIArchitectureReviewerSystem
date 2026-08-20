namespace AIArchitectureReviewer.Application.DTOs.Requests
{
    public class CreateSystemRuleDto
    {
        public string DiagramType { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string RegexOrCondition { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
