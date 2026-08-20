using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiagramManager.API.Controllers;

[ApiController]
[Route("api/workspaces")]
public class WorkspacesController : ControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspacesController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    /// <summary>
    /// Creates a new workspace (Returns 201 Created on success).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _workspaceService.CreateWorkspaceAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Data?.Id },
            response);
    }

    /// <summary>
    /// Updates an existing workspace (Returns 200 OK on success, 404 Not Found if workspace does not exist).
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkspaceRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _workspaceService.UpdateWorkspaceAsync(id, request, cancellationToken);
        if (!response.Success)
        {
            return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(response)
                : BadRequest(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets a workspace by ID (Returns 200 OK on success, 404 Not Found if not found).
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _workspaceService.GetWorkspaceByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets paginated workspaces for a specific user (Returns 200 OK).
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserWorkspaces(Guid userId, [FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var response = await _workspaceService.GetUserWorkspacesAsync(userId, paginationParams, cancellationToken);
        return Ok(response);
    }
}