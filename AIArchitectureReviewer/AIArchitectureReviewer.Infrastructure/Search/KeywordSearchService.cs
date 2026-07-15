using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Search;
using AIArchitectureReviewer.Application.Interfaces.Search;
using AIArchitectureReviewer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIArchitectureReviewer.Infrastructure.Search
{
    public class KeywordSearchService : IKeywordSearchService
    {
        private readonly ApplicationDbContext _dbContext;

        public KeywordSearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SearchResultDTO>> SearchKeywordAsync(string query, int topK)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<SearchResultDTO>();
            }

            var searchTerm = $"%{query}%";

            // Simple Lexical Search using ILike.
            // For a production system, consider PostgreSQL Full-Text Search (ToTsVector / plainto_tsquery).
            var dbQuery = _dbContext.RuleChunks
                .Where(c => EF.Functions.ILike(c.Content, searchTerm))
                .Take(topK)
                .Select(c => new SearchResultDTO
                {
                    Id = c.Id.ToString(),
                    Title = c.SystemRule != null ? c.SystemRule.RuleName : "Rule Chunk",
                    Content = c.Content,
                    Score = 1.0, // Base score for keyword match, could be improved with FTS ranking
                    Source = "keyword"
                });

            return await dbQuery.ToListAsync();
        }
    }
}
