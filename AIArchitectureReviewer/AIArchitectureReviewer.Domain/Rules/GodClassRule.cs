using AIArchitectureReviewer.Domain.Models;

namespace AIArchitectureReviewer.Domain.Rules
{
    public class GodClassRule : IArchitectureRule
    {
        public string Name => "God Class Rule";

        public RuleResult Evaluate(object architectureModel)
        {
            // TODO: Check for classes with too many responsibilities (SRP violation)
            return new RuleResult { RuleName = Name, IsPassed = true, Message = "Checked" };
        }
    }
}
