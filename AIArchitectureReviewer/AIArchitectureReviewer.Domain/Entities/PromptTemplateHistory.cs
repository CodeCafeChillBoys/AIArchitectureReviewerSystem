using System;

namespace AIArchitectureReviewer.Domain.Entities
{
    /// <summary>
    /// Một field đã đổi trong một lần cập nhật PromptTemplate.
    /// Các dòng cùng một lần PUT dùng chung ChangeSetId.
    /// </summary>
    public class PromptTemplateHistory : IChangeHistoryRow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ChangeSetId { get; set; }
        public Guid PromptTemplateId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? UnifiedDiff { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public Guid? ChangedBy { get; set; }

        public PromptTemplate? PromptTemplate { get; set; }
    }
}
