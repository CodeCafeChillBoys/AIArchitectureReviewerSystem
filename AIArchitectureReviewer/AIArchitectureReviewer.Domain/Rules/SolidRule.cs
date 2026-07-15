using AIArchitectureReviewer.Domain.Models;

namespace AIArchitectureReviewer.Domain.Rules
{
    public class SolidRule : IArchitectureRule
    {
        public string Name => "SOLID Principles Rule";

        public RuleResult Evaluate(object architectureModel)
        {
            // TODO: Check general SOLID violations structurally
            return new RuleResult { RuleName = Name, IsPassed = true, Message = "Checked" };
        }
    }
}
