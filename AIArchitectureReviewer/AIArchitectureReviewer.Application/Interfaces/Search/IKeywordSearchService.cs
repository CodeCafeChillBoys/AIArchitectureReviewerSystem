using System.Collections.Generic;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;

namespace AIArchitectureReviewer.Application.Interfaces.Search
{
    public interface IKeywordSearchService
    {
        Task<IEnumerable<SearchResultDTO>> SearchKeywordAsync(string query, int topK);
    }
}
