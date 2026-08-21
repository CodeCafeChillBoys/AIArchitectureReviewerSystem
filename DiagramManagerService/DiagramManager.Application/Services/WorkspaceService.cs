using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiagramManager.Application.Constants;
using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.DTOs.Response;
using DiagramManager.Application.Interfaces;
using DiagramManager.Domain.Entities;
using DiagramManager.Domain.Interfaces;

namespace DiagramManager.Application.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkspaceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkspaceResponseDto>> CreateWorkspaceAsync(CreateWorkspaceRequestDto request, CancellationToken cancellationToken = default)
        {
            var entity = new Workspace
            {
                Name = request.Name,
                UserId = request.UserId
            };

            await _unitOfWork.Repository<Workspace>().AddAsync(entity, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(new WorkspaceResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
                DiagramCount = 0
            }, DiagramMessages.WorkspaceCreatedSuccess);
        }

        public async Task<ApiResponse<PagedResponse<WorkspaceResponseDto>>> GetUserWorkspacesAsync(
            Guid userId,
            PaginationParams paginationParams,
            CancellationToken cancellationToken = default)
        {
            var workspaces = (await _unitOfWork.Repository<Workspace>().FindAsync(w => w.UserId == userId, cancellationToken)).ToList();
            var diagrams = (await _unitOfWork.Repository<Diagram>().GetAllAsync(cancellationToken)).ToList();

            var totalCount = workspaces.Count;
            var pagedItems = workspaces
                .OrderByDescending(w => w.CreatedAt)
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(w => new WorkspaceResponseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    UserId = w.UserId,
                    CreatedAt = w.CreatedAt,
                    DiagramCount = diagrams.Count(d => d.WorkspaceId == w.Id)
                })
                .ToList();

            var pagedResult = new PagedResponse<WorkspaceResponseDto>(
                pagedItems,
                totalCount,
                paginationParams.PageIndex,
                paginationParams.PageSize);

            return ApiResponse<PagedResponse<WorkspaceResponseDto>>.SuccessResponse(pagedResult);
        }

        public async Task<ApiResponse<WorkspaceResponseDto>> GetWorkspaceByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var workspace = await _unitOfWork.Repository<Workspace>().GetByIdAsync(id, cancellationToken);
            if (workspace == null)
            {
                return ApiResponse<WorkspaceResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }

            var diagrams = await _unitOfWork.Repository<Diagram>().FindAsync(d => d.WorkspaceId == id, cancellationToken);

            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(new WorkspaceResponseDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                UserId = workspace.UserId,
                CreatedAt = workspace.CreatedAt,
                DiagramCount = diagrams.Count()
            });
        }

        public async Task<ApiResponse<WorkspaceResponseDto>> UpdateWorkspaceAsync(Guid id, UpdateWorkspaceRequestDto request, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.Repository<Workspace>();
            var workspace = await repo.GetByIdAsync(id, cancellationToken);
            if (workspace == null)
            {
                return ApiResponse<WorkspaceResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }

            workspace.Name = request.Name;
            repo.Update(workspace);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var diagrams = await _unitOfWork.Repository<Diagram>().FindAsync(d => d.WorkspaceId == id, cancellationToken);

            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(new WorkspaceResponseDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                UserId = workspace.UserId,
                CreatedAt = workspace.CreatedAt,
                DiagramCount = diagrams.Count()
            }, DiagramMessages.WorkspaceUpdatedSuccess);
        }
    }
}