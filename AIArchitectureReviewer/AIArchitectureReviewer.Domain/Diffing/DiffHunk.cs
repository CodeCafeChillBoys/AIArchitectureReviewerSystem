using System.Collections.Generic;

namespace AIArchitectureReviewer.Domain.Diffing
{
    /// <summary>
    /// Một khối thay đổi liên tiếp kèm dòng ngữ cảnh, tương ứng một khối @@ trong unified diff.
    /// OldStart/OldCount bằng 0 khi hunk chỉ toàn dòng thêm mới.
    /// </summary>
    public sealed record DiffHunk(
        int OldStart,
        int OldCount,
        int NewStart,
        int NewCount,
        IReadOnlyList<DiffLine> Lines);
}
