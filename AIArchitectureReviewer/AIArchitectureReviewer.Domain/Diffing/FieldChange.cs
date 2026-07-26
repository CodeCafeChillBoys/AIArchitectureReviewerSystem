namespace AIArchitectureReviewer.Domain.Diffing
{
    /// <summary>
    /// Một field đã thay đổi. Field ngắn dùng OldValue/NewValue và để UnifiedDiff null.
    /// Field text dài dùng UnifiedDiff và để OldValue/NewValue null.
    /// </summary>
    public sealed record FieldChange(
        string Field,
        string? OldValue,
        string? NewValue,
        string? UnifiedDiff,
        int Additions,
        int Deletions);
}
