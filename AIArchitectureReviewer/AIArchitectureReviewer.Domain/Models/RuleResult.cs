namespace AIArchitectureReviewer.Domain.Models
{
    public class RuleResult
    {
        public string RuleName { get; set; } = string.Empty;
        public bool IsPassed { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
