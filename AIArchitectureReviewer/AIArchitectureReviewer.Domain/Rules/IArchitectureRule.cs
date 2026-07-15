using AIArchitectureReviewer.Domain.Models;

namespace AIArchitectureReviewer.Domain.Rules
{
    public interface IArchitectureRule
    {
        string Name { get; }
        // TODO: Replace 'object' with an actual Diagram/Architecture model
        RuleResult Evaluate(object architectureModel);
    }
}
