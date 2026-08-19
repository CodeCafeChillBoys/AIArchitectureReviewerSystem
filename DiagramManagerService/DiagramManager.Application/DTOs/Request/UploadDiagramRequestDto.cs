using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiagramManager.Application.DTOs.Request
{
    public class UploadDiagramRequestDto
    {
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DiagramType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile ImageFile { get; set; } = null!;
    }
}