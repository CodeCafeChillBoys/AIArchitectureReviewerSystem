using System;

namespace DiagramManager.Application.DTOs
{
    public class ExtractedDiagramDto
    {
        public byte[] ImageBytes { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int PageNumber { get; set; }
    }
}
