namespace AIArchitectureReviewer.Domain.Diffing
{
    /// <summary>
    /// Một dòng trong kết quả diff. OldLineNo null nghĩa là dòng này chỉ có ở bản mới,
    /// NewLineNo null nghĩa là dòng này chỉ có ở bản cũ. Số dòng đánh từ 1.
    /// </summary>
    public sealed record DiffLine(DiffOp Op, string Text, int? OldLineNo, int? NewLineNo);
}
