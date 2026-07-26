using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DiagramManager.Application.DTOs;
using DiagramManager.Application.Helpers;
using DiagramManager.Application.Interfaces;
using DiagramManager.Domain.Entities;
using DiagramManager.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Messaging.Events;

namespace DiagramManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly WorkspaceDbContext _context;
        private readonly IFileStorageService _storageService;
        private readonly IDocumentExtractorService _extractorService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IDistributedCache _cache;

        public DocumentsController(
            WorkspaceDbContext context,
            IFileStorageService storageService,
            IDocumentExtractorService extractorService,
            IPublishEndpoint publishEndpoint,
            IDistributedCache cache)
        {
            _context = context;
            _storageService = storageService;
            _extractorService = extractorService;
            _publishEndpoint = publishEndpoint;
            _cache = cache;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] Guid workspaceId, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty");

            var workspace = await _context.Workspaces.FindAsync(workspaceId);
            if (workspace == null) return NotFound("Workspace not found");

            // 1. Lưu file tài liệu gốc (PDF/Word)
            string documentStorageUrl = await _storageService.SaveFileAsync(file, $"{workspaceId}/documents");

            // 2. Tạo đối tượng Document trong DB
            var document = new Document
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                Name = file.FileName,
                StorageUrl = documentStorageUrl,
                UploadedAt = DateTime.UtcNow
            };
            _context.Documents.Add(document);

            // 3. Tiến hành trích xuất sơ đồ bằng DocumentExtractorService
            List<ExtractedDiagramDto> extractedDiagrams;
            using (var fileStream = file.OpenReadStream())
            {
                extractedDiagrams = await _extractorService.ExtractDiagramsAsync(fileStream, file.FileName);
            }

            var responseDiagrams = new List<object>();

            // 4. Lưu từng sơ đồ được trích xuất
            foreach (var ext in extractedDiagrams)
            {
                // Xác định đuôi file dựa trên MimeType
                string extension = ext.MimeType == "image/jpeg" ? ".jpg" : ".png";
                string imageFileName = $"{ext.Name}{extension}";

                // Tạo đối tượng IFormFile ảo từ mảng byte để lưu qua StorageService
                var memoryStream = new MemoryStream(ext.ImageBytes);
                var mockFormFile = new FormFile(memoryStream, 0, ext.ImageBytes.Length, "file", imageFileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = ext.MimeType
                };

                // Lưu ảnh sơ đồ
                string diagramStorageUrl = await _storageService.SaveFileAsync(mockFormFile, $"{workspaceId}/extracted");

                // Tính SHA256 Hash của ảnh sơ đồ được trích xuất từ Document
                string imageHash = FileHashHelper.ComputeSha256(ext.ImageBytes);
                string hashCacheKey = $"diagram:hash:{imageHash}";

                // Kiểm tra xem Redis Cache đã có kết quả AI Review của ảnh này chưa
                string? cachedReviewJson = await _cache.GetStringAsync(hashCacheKey);

                // Lưu thực thể Diagram mới liên kết với Document
                var diagram = new Diagram
                {
                    Id = Guid.NewGuid(),
                    WorkspaceId = workspaceId,
                    DocumentId = document.Id,
                    Name = ext.Name,
                    Description = $"Sơ đồ trích xuất từ tài liệu {file.FileName} (Trang {ext.PageNumber})",
                    CreatedAt = DateTime.UtcNow
                };

                DiagramVersion version;

                if (!string.IsNullOrEmpty(cachedReviewJson))
                {
                    // CACHE HIT: Ảnh trong PDF trùng khớp với sơ đồ đã được AI review trước đó!
                    using var docDoc = JsonDocument.Parse(cachedReviewJson);
                    var root = docDoc.RootElement;

                    float? score = root.TryGetProperty("score", out var sProp) && sProp.ValueKind != JsonValueKind.Null ? (float?)sProp.GetDouble() : null;
                    string? reviewData = root.TryGetProperty("reviewData", out var rProp) ? rProp.GetString() : null;
                    string? diagramType = root.TryGetProperty("diagramType", out var dtProp) ? dtProp.GetString() : null;

                    version = new DiagramVersion
                    {
                        Id = Guid.NewGuid(),
                        DiagramId = diagram.Id,
                        VersionNumber = 1,
                        StorageUrl = diagramStorageUrl,
                        RawFormat = extension,
                        AiScore = score,
                        AiReview = reviewData,
                        DiagramType = diagramType,
                        Status = "Analyzed",
                        UploadedAt = DateTime.UtcNow
                    };

                    _context.Diagrams.Add(diagram);
                    _context.DiagramVersions.Add(version);

                    responseDiagrams.Add(new
                    {
                        DiagramId = diagram.Id,
                        VersionId = version.Id,
                        Name = diagram.Name,
                        StorageUrl = diagramStorageUrl,
                        IsDuplicate = true,
                        FromCache = true,
                        Message = "Sơ đồ này trùng khớp với file đã có trong Redis Cache",
                        AiScore = score,
                        AiReview = reviewData,
                        DiagramType = diagramType
                    });
                }
                else
                {
                    // CACHE MISS: Ảnh mới, tạo version với status "Uploaded" và gửi vào RabbitMQ
                    version = new DiagramVersion
                    {
                        Id = Guid.NewGuid(),
                        DiagramId = diagram.Id,
                        VersionNumber = 1,
                        StorageUrl = diagramStorageUrl,
                        RawFormat = extension,
                        Status = "Uploaded",
                        UploadedAt = DateTime.UtcNow
                    };

                    _context.Diagrams.Add(diagram);
                    _context.DiagramVersions.Add(version);

                    // Lưu VersionId -> ImageHash mapping vào Redis 24h
                    await _cache.SetStringAsync($"diagram:version_hash:{version.Id}", imageHash, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                    });

                    // 5. Publish sự kiện để kích hoạt AI chấm điểm từng sơ đồ riêng lẻ
                    await _publishEndpoint.Publish(new DiagramUploadedEvent(
                        diagram.Id,
                        version.Id,
                        version.StorageUrl,
                        ext.ImageBytes
                    ));

                    responseDiagrams.Add(new
                    {
                        DiagramId = diagram.Id,
                        VersionId = version.Id,
                        Name = diagram.Name,
                        StorageUrl = diagramStorageUrl,
                        IsDuplicate = false,
                        FromCache = false,
                        FileHash = imageHash
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                DocumentId = document.Id,
                DocumentName = document.Name,
                StorageUrl = documentStorageUrl,
                ExtractedDiagrams = responseDiagrams
            });
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetDocumentsByWorkspace(Guid workspaceId)
        {
            var docs = await _context.Documents
                .Where(d => d.WorkspaceId == workspaceId)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();
            return Ok(docs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(Guid id)
        {
            var doc = await _context.Documents
                .Include(d => d.Diagrams)
                    .ThenInclude(diag => diag.Versions)
                .FirstOrDefaultAsync(d => d.Id == id);
            
            if (doc == null) return NotFound();
            return Ok(doc);
        }
    }
}