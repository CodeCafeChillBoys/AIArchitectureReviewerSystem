using DiagramManager.Application.DTOs;
using DiagramManager.Application.Interfaces;
using DiagramManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiagramManager.API.Controllers;

[ApiController]
[Route("api/admin/storage")]
[Authorize(Policy = "AdminOnly")]
public class AdminStorageController : ControllerBase
{
    private readonly WorkspaceDbContext _context;
    private readonly IStorageMonitoringService _storageMonitoring;

    public AdminStorageController(
        WorkspaceDbContext context,
        IStorageMonitoringService storageMonitoring)
    {
        _context = context;
        _storageMonitoring = storageMonitoring;
    }

    [HttpGet]
    public async Task<ActionResult<StorageSummaryDto>> GetStorage(
        [FromQuery] int workspaceLimit = 20,
        CancellationToken cancellationToken = default)
    {
        if (workspaceLimit is < 1 or > 100)
        {
            return BadRequest(new
            {
                Message = "workspaceLimit must be between 1 and 100."
            });
        }

        var documentUrls = await _context.Documents
            .AsNoTracking()
            .Select(document => document.StorageUrl)
            .ToListAsync(cancellationToken);
        var diagramUrls = await _context.DiagramVersions
            .AsNoTracking()
            .Select(version => version.StorageUrl)
            .ToListAsync(cancellationToken);

        documentUrls.AddRange(diagramUrls);
        return Ok(_storageMonitoring.GetSummary(documentUrls, workspaceLimit));
    }
}
