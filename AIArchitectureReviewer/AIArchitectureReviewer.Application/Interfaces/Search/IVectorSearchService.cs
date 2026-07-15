using System.Collections.Generic;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;

namespace AIArchitectureReviewer.Application.Interfaces.Search
{
    public interface IVectorSearchService
    {
        Task<IEnumerable<SearchResultDTO>> SearchVectorAsync(float[] vector, int topK);
    }
}
