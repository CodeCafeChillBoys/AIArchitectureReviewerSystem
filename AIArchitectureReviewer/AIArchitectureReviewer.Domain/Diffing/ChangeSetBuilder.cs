using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Diffing
{
    /// <summary>
    /// Dựng ChangeSet bằng two-way diff: A là giá trị hiện có, B là giá trị client gửi lên.
    /// B là null nghĩa là client không gửi field đó, giữ nguyên giá trị cũ.
    /// </summary>
    public sealed class ChangeSetBuilder
    {
        private readonly List<FieldChange> _changes = new();

        /// <summary>Field ngắn: lưu thẳng giá trị cũ và mới.</summary>
        public ChangeSetBuilder Scalar(string field, string? currentValue, string? incomingValue)
        {
            if (incomingValue is null) return this;
            if (string.Equals(currentValue, incomingValue, StringComparison.Ordinal)) return this;

            _changes.Add(new FieldChange(field, currentValue, incomingValue, null, 0, 0));
            return this;
        }

        /// <summary>Field text dài: lưu unified diff theo dòng thay vì cả khối text.</summary>
        /// <remarks>
        /// currentValue null và currentValue "" được coi là như nhau: với field text dài,
        /// "chưa có nội dung" và "nội dung rỗng" là một trạng thái. Đây là chủ đích, không phải
        /// thiếu sót — nếu tách hai trạng thái này ra thì sẽ sinh bản ghi lịch sử có diff rỗng.
        /// Khác với Scalar(), nơi null và "" là hai giá trị phân biệt.
        ///
        /// Tương tự, hai text chỉ khác nhau ở ký tự xuống dòng cuối cùng cũng được coi là không đổi,
        /// vì SplitLines không sinh dòng rỗng thừa ở cuối — "a\nb\n" và "a\nb" là cùng hai dòng.
        /// </remarks>
        public ChangeSetBuilder Text(string field, string? currentValue, string? incomingValue)
        {
            if (incomingValue is null) return this;

            // Đường tắt: chuỗi đã chuẩn hoá bằng nhau thì chắc chắn không đổi,
            // khỏi phải chạy LCS trên tài liệu dài.
            if (string.Equals(TextDiffer.Normalize(currentValue),
                              TextDiffer.Normalize(incomingValue),
                              StringComparison.Ordinal)) return this;

            var lines = TextDiffer.Diff(currentValue, incomingValue);

            int additions = 0, deletions = 0;
            foreach (var line in lines)
            {
                if (line.Op == DiffOp.Insert) additions++;
                else if (line.Op == DiffOp.Delete) deletions++;
            }

            // Chuỗi khác nhau nhưng không dòng nào được thêm hay bớt: hai text chỉ khác ở
            // ký tự xuống dòng cuối cùng. Đó không phải thay đổi — ghi nhận nó sẽ tạo ra bản ghi
            // lịch sử có diff rỗng và, với system rule, kéo theo cả một lần sinh lại embedding vô ích.
            if (additions == 0 && deletions == 0) return this;

            var unifiedDiff = TextDiffer.ToUnifiedDiff(TextDiffer.ToHunks(lines));
            _changes.Add(new FieldChange(field, null, null, unifiedDiff, additions, deletions));
            return this;
        }

        public ChangeSet Build() => new ChangeSet(Guid.NewGuid(), _changes);
    }
}
