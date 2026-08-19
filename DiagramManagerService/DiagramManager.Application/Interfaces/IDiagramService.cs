using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.DTOs.Response;

namespace DiagramManager.Application.Interfaces;

public interface IDiagramService
{
    Task<ApiResponse<DiagramResponseDto>> UploadDiagramAsync(UploadDiagramRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<DiagramResponseDto>> GetDiagramByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResponse<DiagramResponseDto>>> GetWorkspaceDiagramsAsync(Guid workspaceId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<ApiResponse<DiagramResponseDto>> CreateMermaidDiagramAsync(CreateMermaidDiagramRequestDto request, CancellationToken cancellationToken = default);
}
