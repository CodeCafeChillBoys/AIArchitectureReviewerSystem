using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Application.DTOs
{
    /// <summary>Kết quả trả về của một lần PUT.</summary>
    public class ChangeSetDto
    {
        public Guid Id { get; set; }
        public Guid ChangeSetId { get; set; }
        public bool HasChanges { get; set; }
        public List<FieldChangeDto> Changes { get; set; } = new();
    }
}
