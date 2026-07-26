namespace AIArchitectureReviewer.Application.DTOs
{
    /// <summary>
    /// Field null nghĩa là client không gửi, giữ nguyên giá trị hiện có.
    /// Lưu ý đổi hành vi: trước đây bỏ trống IsActive nghĩa là false,
    /// từ nay nghĩa là giữ nguyên. Muốn tắt rule phải gửi tường minh false.
    /// </summary>
    public class UpdateSystemRuleDto
    {
        public string? DiagramType { get; set; }
        public string? RuleName { get; set; }
        public string? RegexOrCondition { get; set; }
        public bool? IsActive { get; set; }
    }
}
