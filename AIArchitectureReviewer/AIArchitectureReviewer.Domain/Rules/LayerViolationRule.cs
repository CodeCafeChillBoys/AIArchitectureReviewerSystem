using AIArchitectureReviewer.Domain.Models;

namespace AIArchitectureReviewer.Domain.Rules
{
    public class LayerViolationRule : IArchitectureRule
    {
        public string Name => "Layer Violation Rule";

        public RuleResult Evaluate(object architectureModel)
        {
            // TODO: Implement static analysis to check if Domain depends on Infrastructure, etc.
            return new RuleResult { RuleName = Name, IsPassed = true, Message = "Checked" };
        }
    }
}
