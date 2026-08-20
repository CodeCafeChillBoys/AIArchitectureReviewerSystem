using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiagramManager.Application.Constants;
using DiagramManager.Application.DTOs.Events;
using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.DTOs.Response;
using DiagramManager.Application.Interfaces;
using DiagramManager.Domain.Entities;
using DiagramManager.Domain.Enums;
using DiagramManager.Domain.Interfaces;
using MassTransit;
namespace DiagramManager.Application.Services
{
    public class DiagramService : IDiagramService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public DiagramService(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ApiResponse<DiagramResponseDto>> CreateMermaidDiagramAsync(CreateMermaidDiagramRequestDto request, CancellationToken cancellationToken = default)
        {
            var workspaceRepo = _unitOfWork.Repository<Workspace>();
            var workspace = await workspaceRepo.GetByIdAsync(request.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                return ApiResponse<DiagramResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diagrams");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = $"{Guid.NewGuid()}_diagram.mmd";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // Ghi mã văn bản Mermaid vào file .mmd
            await File.WriteAllTextAsync(filePath, request.MermaidCode, cancellationToken);
            var storageUrl = $"/uploads/diagrams/{uniqueFileName}";
            // 3. Khởi tạo Diagram & DiagramVersion với RawFormat = "mermaid"
            var diagram = new Diagram
            {
                WorkspaceId = request.WorkspaceId,
                Name = request.Name,
                DiagramType = request.DiagramType,
                Description = request.Description
            };
            var diagramRepo = _unitOfWork.Repository<Diagram>();
            await diagramRepo.AddAsync(diagram, cancellationToken);
            var version = new DiagramVersion
            {
                DiagramId = diagram.Id,
                VersionNumber = 1,
                StorageUrl = storageUrl,
                RawFormat = "mermaid",
                Status = DiagramVersionStatus.Pending
            };
            var versionRepo = _unitOfWork.Repository<DiagramVersion>();
            await versionRepo.AddAsync(version, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _publishEndpoint.Publish(new DiagramProcessingRequestEvent
            {
                DiagramId = diagram.Id,
                DiagramVersionId = version.Id,
                DiagramType = diagram.DiagramType,
                RawFormat = "mermaid",
                StorageUrl = filePath,
                ContentText = request.MermaidCode
            }, cancellationToken);
            // 4. Trả về Response
            var responseDto = new DiagramResponseDto
            {
                Id = diagram.Id,
                WorkspaceId = diagram.WorkspaceId,
                Name = diagram.Name,
                DiagramType = diagram.DiagramType,
                Description = diagram.Description,
                CurrentStorageUrl = version.StorageUrl,
                CurrentVersion = version.VersionNumber,
                CurrentVersionId = version.Id,
                ContentText = request.MermaidCode,
                CurrentStatus = version.Status,
                CreatedAt = diagram.CreatedAt
            };
            return ApiResponse<DiagramResponseDto>.SuccessResponse(responseDto, DiagramMessages.DiagramCreatedSuccess);
        }

        public async Task<ApiResponse<DiagramResponseDto>> GetDiagramByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var diagramRepo = _unitOfWork.Repository<Diagram>();
            var diagram = await diagramRepo.GetByIdAsync(id, cancellationToken);
            if (diagram == null)
            {
                return ApiResponse<DiagramResponseDto>.FailureResponse(DiagramMessages.DiagramNotFound);
            }
            var versionRepo = _unitOfWork.Repository<DiagramVersion>();
            var versions = await versionRepo.FindAsync(v => v.DiagramId == id, cancellationToken);
            var latestVersion = versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
            
            string? contentText = null;
            if (latestVersion != null && !string.IsNullOrEmpty(latestVersion.StorageUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", latestVersion.StorageUrl.TrimStart('/', '\\'));
                if (File.Exists(filePath))
                {
                    contentText = await File.ReadAllTextAsync(filePath, cancellationToken);
                }
            }

            var responseDto = new DiagramResponseDto
            {
                Id = diagram.Id,
                WorkspaceId = diagram.WorkspaceId,
                Name = diagram.Name,
                DiagramType = diagram.DiagramType,
                Description = diagram.Description,
                CurrentStorageUrl = latestVersion?.StorageUrl ?? string.Empty,
                CurrentVersion = latestVersion?.VersionNumber ?? 1,
                CurrentVersionId = latestVersion?.Id,
                ContentText = contentText,
                CurrentStatus = latestVersion?.Status ?? DiagramVersionStatus.Pending,
                CreatedAt = diagram.CreatedAt
            };
            return ApiResponse<DiagramResponseDto>.SuccessResponse(responseDto);

        }

        public async Task<ApiResponse<PagedResponse<DiagramResponseDto>>> GetWorkspaceDiagramsAsync(Guid workspaceId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
        {
            var diagramRepo = _unitOfWork.Repository<Diagram>();
            var diagrams = await diagramRepo.FindAsync(d => d.WorkspaceId == workspaceId, cancellationToken);
            var totalCount = diagrams.Count();
            var versionRepo = _unitOfWork.Repository<DiagramVersion>();
            var pagedItems = new List<DiagramResponseDto>();
            var pagedDiagrams = diagrams
                .OrderByDescending(d => d.CreatedAt)
                .Skip((paginationParams.PageIndex - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize);

            foreach (var diagram in pagedDiagrams)
            {
                var versions = await versionRepo.FindAsync(v => v.DiagramId == diagram.Id, cancellationToken);
                var latestVersion = versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
                pagedItems.Add(new DiagramResponseDto
                {
                    Id = diagram.Id,
                    WorkspaceId = diagram.WorkspaceId,
                    Name = diagram.Name,
                    DiagramType = diagram.DiagramType,
                    Description = diagram.Description,
                    CurrentStorageUrl = latestVersion?.StorageUrl ?? string.Empty,
                    CurrentVersion = latestVersion?.VersionNumber ?? 1,
                    CurrentVersionId = latestVersion?.Id,
                    CurrentStatus = latestVersion?.Status ?? DiagramVersionStatus.Pending,
                    CreatedAt = diagram.CreatedAt
                });
            }
            var pagedResult = new PagedResponse<DiagramResponseDto>(
                pagedItems,
                totalCount,
                paginationParams.PageIndex,
                paginationParams.PageSize);
            return ApiResponse<PagedResponse<DiagramResponseDto>>.SuccessResponse(pagedResult);
        }

        public async Task<ApiResponse<DiagramResponseDto>> UploadDiagramAsync(UploadDiagramRequestDto request, CancellationToken cancellationToken = default)
        {
            var workspaceRepo = _unitOfWork.Repository<Workspace>();
            var existingWorkspace = await workspaceRepo.GetByIdAsync(request.WorkspaceId, cancellationToken);
            if (existingWorkspace == null)
            {
                return ApiResponse<DiagramResponseDto>.FailureResponse(DiagramMessages.WorkspaceNotFound);
            }
            var rawFormat = Path.GetExtension(request.ImageFile.FileName).TrimStart('.').ToLower();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "diagrams");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = $"{Guid.NewGuid()}_{request.ImageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream, cancellationToken);
            }
            var storageUrl = $"/uploads/diagrams/{uniqueFileName}";

            var diagram = new Diagram
            {
                WorkspaceId = request.WorkspaceId,
                Name = request.Name,
                DiagramType = request.DiagramType,
                Description = request.Description
            };
            var diagramRepo = _unitOfWork.Repository<Diagram>();
            await diagramRepo.AddAsync(diagram, cancellationToken);
            var version = new DiagramVersion
            {
                DiagramId = diagram.Id,
                VersionNumber = 1,
                StorageUrl = storageUrl,
                RawFormat = rawFormat,
                Status = DiagramVersionStatus.Pending
            };

            var versionRepo = _unitOfWork.Repository<DiagramVersion>();
            await versionRepo.AddAsync(version, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            await _publishEndpoint.Publish(new DiagramProcessingRequestEvent
            {
                DiagramId = diagram.Id,
                DiagramVersionId = version.Id,
                DiagramType = diagram.DiagramType,
                RawFormat = rawFormat,
                StorageUrl = filePath
            }, cancellationToken);
            var responseDto = new DiagramResponseDto
            {
                Id = diagram.Id,
                WorkspaceId = diagram.WorkspaceId,
                Name = diagram.Name,
                DiagramType = diagram.DiagramType,
                Description = diagram.Description,
                CurrentStorageUrl = version.StorageUrl,
                CurrentVersion = version.VersionNumber,
                CurrentStatus = version.Status,
                CreatedAt = diagram.CreatedAt
            };
            return ApiResponse<DiagramResponseDto>.SuccessResponse(responseDto, DiagramMessages.DiagramCreatedSuccess);
        }
    }
}