using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagramManager.Application.DTOs.Request
{
    public class CreateMermaidDiagramRequestDto
    {
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DiagramType { get; set; } = string.Empty; // VD: "Flowchart", "Sequence", "Class"
        public string Description { get; set; } = string.Empty;

        // Chuỗi mã Mermaid Code (VD: "graph TD\n A --> B")
        public string MermaidCode { get; set; } = string.Empty;
    }
}