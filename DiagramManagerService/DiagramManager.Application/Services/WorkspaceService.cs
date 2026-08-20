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

            var workSpaceRepo = _unitOfWork.Repository<Workspace>();
            await workSpaceRepo.AddAsync(entity, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            var responseDto = new WorkspaceResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt
            };
            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(responseDto, DiagramMessages.WorkspaceCreatedSuccess);
        }

        public async Task<ApiResponse<PagedResponse<WorkspaceResponseDto>>> GetUserWorkspacesAsync(
        Guid userId,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
        {
            var workspaceRepo = _unitOfWork.Repository<Workspace>();
            var userWorkspaces = await workspaceRepo.FindAsync(w => w.UserId == userId, cancellationToken);
            var totalCount = userWorkspaces.Count();
            var pagedItems = userWorkspaces
    .OrderByDescending(w => w.CreatedAt)
    .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
    .Take(paginationParams.PageSize)
    .Select(w => new WorkspaceResponseDto
    {
        Id = w.Id,
        Name = w.Name,
        UserId = w.UserId,
        CreatedAt = w.CreatedAt
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
            var workspaceRepo = _unitOfWork.Repository<Workspace>();

            var workspace = await workspaceRepo.GetByIdAsync(id, cancellationToken);
            if (workspace == null)
            {
                return ApiResponse<WorkspaceResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }

            var responseDto = new WorkspaceResponseDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                UserId = workspace.UserId,
                CreatedAt = workspace.CreatedAt
            };

            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(responseDto);
        }

        public async Task<ApiResponse<WorkspaceResponseDto>> UpdateWorkspaceAsync(Guid id, UpdateWorkspaceRequestDto request, CancellationToken cancellationToken = default)
        {
            var workSpaceRepo = _unitOfWork.Repository<Workspace>();
            var existingWorkspace = await workSpaceRepo.GetByIdAsync(id, cancellationToken);
            if (existingWorkspace == null)
            {
                return ApiResponse<WorkspaceResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }
            existingWorkspace.Name = request.Name;
            workSpaceRepo.Update(existingWorkspace);
            await _unitOfWork.CompleteAsync(cancellationToken);
            var responseDto = new WorkspaceResponseDto
            {
                Id = existingWorkspace.Id,
                Name = existingWorkspace.Name,
                UserId = existingWorkspace.UserId,
                CreatedAt = existingWorkspace.CreatedAt
            };
            return ApiResponse<WorkspaceResponseDto>.SuccessResponse(responseDto, DiagramMessages.WorkspaceUpdatedSuccess);
        }
    }
}