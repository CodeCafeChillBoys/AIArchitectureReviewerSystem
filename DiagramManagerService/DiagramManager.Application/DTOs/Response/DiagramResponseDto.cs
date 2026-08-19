using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiagramManager.Domain.Enums;

namespace DiagramManager.Application.DTOs.Response
{
    public class DiagramResponseDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DiagramType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurrentStorageUrl { get; set; } = string.Empty;
        public int CurrentVersion { get; set; }
        public DiagramVersionStatus CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}