using DiagramManager.Application.DTOs.Request;
using DiagramManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiagramManager.API.Controllers;

[ApiController]
[Route("api/diagrams")]
public class DiagramsController : ControllerBase
{
    private readonly IDiagramService _diagramService;

    public DiagramsController(IDiagramService diagramService)
    {
        _diagramService = diagramService;
    }

    [HttpPost("mermaid")]
    public async Task<IActionResult> CreateMermaid([FromBody] CreateMermaidDiagramRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _diagramService.CreateMermaidDiagramAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadDiagramRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _diagramService.UploadDiagramAsync(request, cancellationToken);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _diagramService.GetDiagramByIdAsync(id, cancellationToken);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }


    [HttpGet("workspace/{workspaceId:guid}")]
    public async Task<IActionResult> GetWorkspaceDiagrams(Guid workspaceId, [FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var response = await _diagramService.GetWorkspaceDiagramsAsync(workspaceId, paginationParams, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDiagram(Guid id, CancellationToken cancellationToken)
    {
        var response = await _diagramService.DeleteDiagramAsync(id, cancellationToken);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }
}
