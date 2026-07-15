using System.Threading.Tasks;
// using AIArchitectureReviewer.Domain.LLM;

namespace AIArchitectureReviewer.Application.Interfaces.AI
{
    public interface ILLMProvider
    {
        string GetModelName();
        // Task<string> GenerateResponseAsync(LLMRequest request, System.Threading.CancellationToken cancellationToken = default);
    }
}
