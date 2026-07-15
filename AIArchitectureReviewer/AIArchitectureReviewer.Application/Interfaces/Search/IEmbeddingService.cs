using System.Threading.Tasks;

namespace AIArchitectureReviewer.Application.Interfaces.Search
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
    }
}
