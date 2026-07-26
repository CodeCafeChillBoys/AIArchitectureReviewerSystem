using System;
using System.Collections.Generic;

namespace AIArchitectureReviewer.Application.DTOs
{
    /// <summary>Một lần thay đổi trong lịch sử, gom theo ChangeSetId.</summary>
    public class ChangeHistoryEntryDto
    {
        public Guid ChangeSetId { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid? ChangedBy { get; set; }
        public List<FieldChangeDto> Changes { get; set; } = new();
    }
}
