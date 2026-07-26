using System;
using AIArchitectureReviewer.Domain.Diffing;

namespace AIArchitectureReviewer.Domain.Entities
{
    public static class ChangeHistoryRowExtensions
    {
        /// <summary>
        /// Điền phần chung của một dòng history từ một FieldChange.
        /// Khoá ngoại do bên gọi tự đặt vì mỗi entity có tên khoá riêng.
        /// ChangedBy để null: service chưa có authentication.
        /// </summary>
        public static T FillFrom<T>(this T row, FieldChange change, Guid changeSetId, DateTime changedAt)
            where T : IChangeHistoryRow
        {
            row.ChangeSetId = changeSetId;
            row.FieldName = change.Field;
            row.OldValue = change.OldValue;
            row.NewValue = change.NewValue;
            row.UnifiedDiff = change.UnifiedDiff;
            row.Additions = change.Additions;
            row.Deletions = change.Deletions;
            row.ChangedAt = changedAt;
            row.ChangedBy = null;
            return row;
        }
    }
}
