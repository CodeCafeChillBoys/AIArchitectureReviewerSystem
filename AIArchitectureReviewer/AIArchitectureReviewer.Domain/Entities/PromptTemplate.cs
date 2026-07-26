using System;
using System.ComponentModel.DataAnnotations;

namespace AIArchitectureReviewer.Domain.Entities
{
    public class PromptTemplate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; 

        [Required]
        public string Content { get; set; } = null!; 

        [Required]
        [MaxLength(50)]
        public string DiagramType { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}