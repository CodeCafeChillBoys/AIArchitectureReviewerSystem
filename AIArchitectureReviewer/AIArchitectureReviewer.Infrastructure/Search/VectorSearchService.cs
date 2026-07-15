using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace AIArchitectureReviewer.Infrastructure.Search
{
    public class VectorSearchService : IVectorSearchService
    {
        private readonly ApplicationDbContext _dbContext;

        public VectorSearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SearchResultDTO>> SearchVectorAsync(float[] vector, int topK)
        {
            if (vector == null || vector.Length == 0)
            {
                return Array.Empty<SearchResultDTO>();
            }

            var pgVector = new Vector(vector);

            var query = _dbContext.RuleChunks
                .Where(c => c.Embedding != null)
                .OrderBy(c => c.Embedding!.CosineDistance(pgVector))
                .Take(topK)
                .Select(c => new SearchResultDTO
                {
                    Id = c.Id.ToString(),
                    Title = c.SystemRule != null ? c.SystemRule.RuleName : "Rule Chunk",
                    Content = c.Content,
                    Score = 1 - c.Embedding!.CosineDistance(pgVector), // Convert distance to similarity score
                    Source = "vector"
                });

            return await query.ToListAsync();
        }
    }
}

