using DiagramManager.Application.Interfaces;
using DiagramManager.Domain.Entities;
using DiagramManager.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Messaging.Events;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiagramManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagramsController : ControllerBase
    {
        private readonly WorkspaceDbContext _context;
        private readonly IFileStorageService _storageService;
        private readonly IPublishEndpoint _publishEndpoint;

        public DiagramsController(WorkspaceDbContext context, IFileStorageService storageService, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _storageService = storageService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDiagram([FromForm] Guid workspaceId, [FromForm] string name, [FromForm] string description, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty");

            var workspace = await _context.Workspaces.FindAsync(workspaceId);
            if (workspace == null) return NotFound("Workspace not found");

            // Save File
            string storageUrl = await _storageService.SaveFileAsync(file, workspaceId.ToString());
            
            // Save to DB
            var diagram = new Diagram
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            var version = new DiagramVersion
            {
                Id = Guid.NewGuid(),
                DiagramId = diagram.Id,
                VersionNumber = 1,
                StorageUrl = storageUrl,
                RawFormat = Path.GetExtension(file.FileName),
                Status = "Uploaded",
                UploadedAt = DateTime.UtcNow
            };

            _context.Diagrams.Add(diagram);
            _context.DiagramVersions.Add(version);
            await _context.SaveChangesAsync();

            // Publish Event
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }
            await _publishEndpoint.Publish(new DiagramUploadedEvent(diagram.Id, version.Id, version.StorageUrl, fileBytes));

            return Ok(new { DiagramId = diagram.Id, VersionId = version.Id, StorageUrl = storageUrl });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiagram(Guid id)
        {
            var diagram = await _context.Diagrams.Include(d => d.Versions).FirstOrDefaultAsync(d => d.Id == id);
            if (diagram == null) return NotFound();
            return Ok(diagram);
        }

        [HttpGet("file")]
        public async Task<IActionResult> DownloadFile([FromQuery] string storageUrl)
        {
            try
            {
                var bytes = await _storageService.ReadFileAsync(storageUrl);
                return File(bytes, "application/octet-stream");
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("workspace/{workspaceId}")]
        public async Task<IActionResult> GetDiagramsByWorkspace(Guid workspaceId)
        {
            var diagrams = await _context.Diagrams.Include(d => d.Versions)
                                                  .Where(d => d.WorkspaceId == workspaceId)
                                                  .ToListAsync();
            return Ok(diagrams);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiagram(Guid id)
        {
            var diagram = await _context.Diagrams.Include(d => d.Versions).FirstOrDefaultAsync(d => d.Id == id);
            if (diagram == null) return NotFound();

            _context.Diagrams.Remove(diagram);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/versions")]
        public async Task<IActionResult> UploadNewVersion(Guid id, [FromForm] string description, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty");

            var diagram = await _context.Diagrams.Include(d => d.Versions).FirstOrDefaultAsync(d => d.Id == id);
            if (diagram == null) return NotFound("Diagram not found");

            if (!string.IsNullOrEmpty(description))
            {
                diagram.Description = description;
            }

            // Save File
            string storageUrl = await _storageService.SaveFileAsync(file, diagram.WorkspaceId.ToString());

            int nextVersionNumber = diagram.Versions.Count > 0 ? diagram.Versions.Max(v => v.VersionNumber) + 1 : 1;

            var version = new DiagramVersion
            {
                Id = Guid.NewGuid(),
                DiagramId = diagram.Id,
                VersionNumber = nextVersionNumber,
                StorageUrl = storageUrl,
                RawFormat = Path.GetExtension(file.FileName),
                Status = "Uploaded",
                UploadedAt = DateTime.UtcNow
            };

            _context.DiagramVersions.Add(version);
            await _context.SaveChangesAsync();

            // Publish Event
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }
            await _publishEndpoint.Publish(new DiagramUploadedEvent(diagram.Id, version.Id, version.StorageUrl, fileBytes));

            return Ok(new { DiagramId = diagram.Id, VersionId = version.Id, VersionNumber = nextVersionNumber, StorageUrl = storageUrl });
        }

        [HttpGet("version/{versionId}")]
        public async Task<IActionResult> GetVersion(Guid versionId)
        {
            var version = await _context.DiagramVersions.FirstOrDefaultAsync(v => v.Id == versionId);
            if (version == null) return NotFound("Version not found");
            return Ok(version);
        }
    }
}
