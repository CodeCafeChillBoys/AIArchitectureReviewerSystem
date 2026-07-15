using AIArchitectureReviewer.Domain.Models;

namespace AIArchitectureReviewer.Domain.Rules
{
    public class CircularDependencyRule : IArchitectureRule
    {
        public string Name => "Circular Dependency Rule";

        public RuleResult Evaluate(object architectureModel)
        {
            // TODO: Detect cycles (A -> B -> A) in module dependencies
            return new RuleResult { RuleName = Name, IsPassed = true, Message = "Checked" };
        }
    }
}
