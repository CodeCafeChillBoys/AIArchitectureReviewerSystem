namespace AIArchitectureReviewer.Application.DTOs
{
    public class FieldChangeDto
    {
        public string Field { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? UnifiedDiff { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
    }
}
