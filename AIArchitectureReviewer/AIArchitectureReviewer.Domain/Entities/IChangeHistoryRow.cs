using System;

namespace AIArchitectureReviewer.Domain.Entities
{
    /// <summary>
    /// Phần chung của một dòng lịch sử thay đổi, không phụ thuộc entity nào.
    /// EF Core bỏ qua interface khi dựng model nên không ảnh hưởng schema.
    /// </summary>
    public interface IChangeHistoryRow
    {
        Guid ChangeSetId { get; set; }
        string FieldName { get; set; }
        string? OldValue { get; set; }
        string? NewValue { get; set; }
        string? UnifiedDiff { get; set; }
        int Additions { get; set; }
        int Deletions { get; set; }
        DateTime ChangedAt { get; set; }
        Guid? ChangedBy { get; set; }
    }
}
