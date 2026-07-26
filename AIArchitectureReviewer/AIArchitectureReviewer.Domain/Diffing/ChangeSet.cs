using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Diffing
{
    /// <summary>
    /// Kết quả của một lần so sánh entity với dữ liệu gửi lên — tương đương một merge request.
    /// </summary>
    public sealed class ChangeSet
    {
        public Guid Id { get; }
        public IReadOnlyList<FieldChange> Changes { get; }
        public bool HasChanges => Changes.Count > 0;

        public ChangeSet(Guid id, IReadOnlyList<FieldChange> changes)
        {
            Id = id;
            Changes = changes;
        }

        public bool Contains(string field)
        {
            for (int i = 0; i < Changes.Count; i++)
            {
                if (string.Equals(Changes[i].Field, field, StringComparison.Ordinal)) return true;
            }
            return false;
        }
    }
}
