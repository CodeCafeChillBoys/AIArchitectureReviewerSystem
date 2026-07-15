using System.Threading.Tasks;

namespace AIArchitectureReviewer.Application.Interfaces.AI
{
    public interface IEmbeddingProvider
    {
        Task<float[]> GetEmbeddingAsync(string text);
    }
}
