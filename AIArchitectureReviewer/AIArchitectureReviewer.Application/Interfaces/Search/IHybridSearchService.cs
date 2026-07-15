using System.Collections.Generic;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;

namespace AIArchitectureReviewer.Application.Interfaces.Search
{
    public interface IHybridSearchService
    {
        Task<IEnumerable<SearchResultDTO>> SearchHybridAsync(string query, int topK);
    }
}
