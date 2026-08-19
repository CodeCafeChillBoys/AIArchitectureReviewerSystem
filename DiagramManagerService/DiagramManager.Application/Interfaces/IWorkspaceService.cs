using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.DTOs.Response;

namespace DiagramManager.Application.Interfaces;

public interface IWorkspaceService
{
    Task<ApiResponse<WorkspaceResponseDto>> CreateWorkspaceAsync(CreateWorkspaceRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<WorkspaceResponseDto>> UpdateWorkspaceAsync(Guid id, UpdateWorkspaceRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<WorkspaceResponseDto>> GetWorkspaceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResponse<WorkspaceResponseDto>>> GetUserWorkspacesAsync(Guid userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
}
