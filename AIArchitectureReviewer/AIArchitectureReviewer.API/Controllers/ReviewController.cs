using System.IO;
using System.Threading.Tasks;
using AIArchitectureReviewer.API.DTOs;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIArchitectureReviewer.Infrastructure.Data;
using AIArchitectureReviewer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AIArchitectureReviewer.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUMLReviewOrchestrator _orchestrator;
        private readonly AIArchitectureReviewer.Application.Interfaces.Services.IChatService _chatService;
        private readonly ApplicationDbContext _context;

        public ReviewController(IUMLReviewOrchestrator orchestrator, AIArchitectureReviewer.Application.Interfaces.Services.IChatService chatService, ApplicationDbContext context)
        {
            _orchestrator = orchestrator;
            _chatService = chatService;
            _context = context;
        }

        [HttpPost("{sessionId}/chat")]
        public async Task<IActionResult> Chat(System.Guid sessionId, [FromBody] AIArchitectureReviewer.Application.DTOs.ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest("Message cannot be empty.");

            try
            {
                var response = await _chatService.SendMessageAsync(sessionId, request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error during chat: {ex.Message}");
            }
        }

        [HttpGet("{sessionId}/chat")]
        public async Task<IActionResult> GetChatHistory(System.Guid sessionId)
        {
            try
            {
                var history = await _chatService.GetMessagesAsync(sessionId);
                return Ok(history);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error while fetching chat history: {ex.Message}");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var history = await _orchestrator.GetReviewHistoryAsync();
                return Ok(history);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error while fetching history: {ex.Message}");
            }
        }

        [HttpPost("consistency")]
        public async Task<IActionResult> CheckConsistency([FromQuery] System.Guid workspaceId, [FromBody] System.Collections.Generic.List<System.Text.Json.Nodes.JsonNode> diagrams)
        {
            if (diagrams == null || diagrams.Count == 0)
                return BadRequest("Diagrams list cannot be empty.");

            try
            {
                var result = await _orchestrator.CheckConsistencyAsync(diagrams);
                
                // Lưu kết quả vào DB nếu có workspaceId
                if (workspaceId != System.Guid.Empty && result != null)
                {
                    var reportText = result["ConsistencyReport"]?.ToString() ?? string.Empty;
                    var diagramNamesList = diagrams
                        .Select(d => d["diagram_name"]?.ToString() ?? d["diagram_type"]?.ToString() ?? "Sơ đồ")
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Distinct();

                    var report = new ConsistencyReport
                    {
                        Id = System.Guid.NewGuid(),
                        WorkspaceId = workspaceId,
                        DiagramNames = string.Join(", ", diagramNamesList),
                        ReportData = reportText,
                        CreatedAt = System.DateTime.UtcNow
                    };

                    _context.ConsistencyReports.Add(report);
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error during consistency check: {ex.Message}");
            }
        }

        [HttpGet("consistency/history/{workspaceId}")]
        public async Task<IActionResult> GetConsistencyHistory(System.Guid workspaceId)
        {
            try
            {
                var history = await _context.ConsistencyReports
                    .Where(r => r.WorkspaceId == workspaceId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return Ok(history);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error getting consistency history: {ex.Message}");
            }
        }

        [HttpGet("versions/{versionId}/report")]
        public async Task<IActionResult> GetReportByVersionId(System.Guid versionId)
        {
            try
            {
                var report = await _orchestrator.GetReportByVersionIdAsync(versionId);
                if (report == null) return NotFound($"No AI report found for diagram version {versionId}.");

                return Ok(report);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error while fetching report: {ex.Message}");
            }
        }

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetHistoryDetail(System.Guid id)
        {
            try
            {
                var detail = await _orchestrator.GetReviewDetailAsync(id);
                if (detail == null) return NotFound($"Review history with ID {id} not found.");


                return Ok(detail);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error while fetching review detail: {ex.Message}");
            }
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteHistoryItem(System.Guid id)
        {
            try
            {
                var report = await _context.AnalysisReports.FindAsync(id);
                if (report == null) return NotFound("Report not found.");

                _context.AnalysisReports.Remove(report);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error deleting report: {ex.Message}");
            }
        }

        [HttpPut("history/{id}/rename")]
        public async Task<IActionResult> RenameHistoryItem(System.Guid id, [FromBody] RenameRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request?.NewName))
                return BadRequest("New name cannot be empty.");

            try
            {
                var report = await _context.AnalysisReports.FindAsync(id);
                if (report == null) return NotFound("Report not found.");

                report.DiagramType = request.NewName;
                _context.AnalysisReports.Update(report);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error renaming report: {ex.Message}");
            }
        }

        [HttpPost("{reportId}/conformance")]
        public async Task<IActionResult> ConformanceReview(System.Guid reportId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var result = await _orchestrator.ConformanceReviewAsync(reportId, stream, file.FileName);
                    return Ok(result);
                }
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error during conformance review: {ex.Message}");
            }
        }
    }

    public class RenameRequestDto
    {
        public string NewName { get; set; } = string.Empty;
    }
}
