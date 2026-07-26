using System;
using System.Collections.Generic;
using System.Linq;
using AIArchitectureReviewer.Application.DTOs;
using AIArchitectureReviewer.Domain.Diffing;
using AIArchitectureReviewer.Domain.Entities;

namespace AIArchitectureReviewer.Application.Mappings
{
    /// <summary>
    /// Chỗ duy nhất chuyển kết quả diff và dòng history sang DTO.
    /// Dùng chung cho PromptTemplate và SystemRule qua IChangeHistoryRow.
    /// </summary>
    public static class ChangeHistoryMapper
    {
        public static FieldChangeDto ToDto(FieldChange change) => new FieldChangeDto
        {
            Field = change.Field,
            OldValue = change.OldValue,
            NewValue = change.NewValue,
            UnifiedDiff = change.UnifiedDiff,
            Additions = change.Additions,
            Deletions = change.Deletions
        };

        public static FieldChangeDto ToDto(IChangeHistoryRow row) => new FieldChangeDto
        {
            Field = row.FieldName,
            OldValue = row.OldValue,
            NewValue = row.NewValue,
            UnifiedDiff = row.UnifiedDiff,
            Additions = row.Additions,
            Deletions = row.Deletions
        };

        /// <summary>Response của một lần PUT có thay đổi.</summary>
        public static ChangeSetDto ToDto(Guid entityId, ChangeSet changeSet) => new ChangeSetDto
        {
            Id = entityId,
            ChangeSetId = changeSet.Id,
            HasChanges = true,
            Changes = changeSet.Changes.Select(ToDto).ToList()
        };

        /// <summary>Response của một lần PUT không đổi gì.</summary>
        /// <remarks>
        /// ChangeSetId trả về Guid.Empty vì không có change set nào được ghi xuống —
        /// trả id do builder sinh ra sẽ khiến client lưu một id không tra cứu được ở đâu cả.
        /// </remarks>
        public static ChangeSetDto NoChanges(Guid entityId, ChangeSet changeSet) => new ChangeSetDto
        {
            Id = entityId,
            ChangeSetId = Guid.Empty,
            HasChanges = false
        };

        /// <summary>Gom các dòng history thành change set, mới nhất trước.</summary>
        public static List<ChangeHistoryEntryDto> ToHistory(IEnumerable<IChangeHistoryRow> rows) =>
            rows.GroupBy(h => h.ChangeSetId)
                .Select(g => new ChangeHistoryEntryDto
                {
                    ChangeSetId = g.Key,
                    ChangedAt = g.Max(h => h.ChangedAt),
                    ChangedBy = g.First().ChangedBy,
                    Changes = g.OrderBy(h => h.FieldName, StringComparer.Ordinal).Select(ToDto).ToList()
                })
                .OrderByDescending(e => e.ChangedAt)
                .ToList();
    }
}
