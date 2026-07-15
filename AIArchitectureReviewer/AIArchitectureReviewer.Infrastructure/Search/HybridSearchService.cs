using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;
using AIArchitectureReviewer.Application.Interfaces.Search;

namespace AIArchitectureReviewer.Infrastructure.Search
{
    public class HybridSearchService : IHybridSearchService
    {
        private readonly IVectorSearchService _vectorSearchService;
        private readonly IKeywordSearchService _keywordSearchService;
        private readonly IEmbeddingService _embeddingService;
        private const int RrfConstant = 60; // Standard constant used in RRF

        public HybridSearchService(
            IVectorSearchService vectorSearchService, 
            IKeywordSearchService keywordSearchService,
            IEmbeddingService embeddingService)
        {
            _vectorSearchService = vectorSearchService;
            _keywordSearchService = keywordSearchService;
            _embeddingService = embeddingService;
        }

        public async Task<IEnumerable<SearchResultDTO>> SearchHybridAsync(string query, int topK)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<SearchResultDTO>();
            }

            // 1. Perform Keyword Search
            var keywordTask = _keywordSearchService.SearchKeywordAsync(query, topK);

            // 2. Perform Vector Search (requires embedding the query first)
            var vectorQuery = await _embeddingService.GenerateEmbeddingAsync(query);
            var vectorTask = _vectorSearchService.SearchVectorAsync(vectorQuery, topK);

            await Task.WhenAll(keywordTask, vectorTask);

            var keywordResults = keywordTask.Result.ToList();
            var vectorResults = vectorTask.Result.ToList();

            // 3. Apply Reciprocal Rank Fusion (RRF)
            var combinedScores = new Dictionary<string, SearchResultDTO>(StringComparer.OrdinalIgnoreCase);

            // Rank keyword results
            for (int i = 0; i < keywordResults.Count; i++)
            {
                var result = keywordResults[i];
                var rank = i + 1;
                var rrfScore = 1.0 / (RrfConstant + rank);

                combinedScores[result.Id] = new SearchResultDTO
                {
                    Id = result.Id,
                    Title = result.Title,
                    Content = result.Content,
                    Score = rrfScore,
                    Source = "hybrid"
                };
            }

            // Rank vector results
            for (int i = 0; i < vectorResults.Count; i++)
            {
                var result = vectorResults[i];
                var rank = i + 1;
                var rrfScore = 1.0 / (RrfConstant + rank);

                if (combinedScores.TryGetValue(result.Id, out var existing))
                {
                    existing.Score += rrfScore; // Add RRF score if it exists in both
                }
                else
                {
                    combinedScores[result.Id] = new SearchResultDTO
                    {
                        Id = result.Id,
                        Title = result.Title,
                        Content = result.Content,
                        Score = rrfScore,
                        Source = "hybrid"
                    };
                }
            }

            // 4. Sort by combined RRF score and return topK
            return combinedScores.Values
                .OrderByDescending(r => r.Score)
                .Take(Math.Max(1, topK))
                .ToList();
        }
    }
}
