using System.ComponentModel.DataAnnotations;

namespace AIArchitectureReviewer.Application.DTOs.Requests
{
    /// <summary>
    /// Field null nghĩa là client không gửi, giữ nguyên giá trị hiện có.
    /// Ràng buộc độ dài khớp với entity PromptTemplate.
    /// </summary>
    public class UpdatePromptTemplateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        public string? Content { get; set; }

        [MaxLength(50)]
        public string? DiagramType { get; set; }
    }
}
