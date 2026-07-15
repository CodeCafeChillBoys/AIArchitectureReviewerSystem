using Pgvector;
using System;
using System.Text.Json.Serialization;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class RuleChunk
    {
        public Guid Id { get; set; }
        public Guid SystemRuleId { get; set; }
        public string Content { get; set; } = string.Empty;
        
        [JsonIgnore]
        public Vector? Embedding { get; set; }

        [JsonIgnore]
        public SystemRule SystemRule { get; set; } = null!;
    }
}
