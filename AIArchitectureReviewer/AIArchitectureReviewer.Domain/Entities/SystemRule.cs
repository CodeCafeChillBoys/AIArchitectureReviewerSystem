using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class SystemRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DiagramType { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string RegexOrCondition { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public ICollection<RuleChunk> RuleChunks { get; set; } = new List<RuleChunk>();
    }
}
