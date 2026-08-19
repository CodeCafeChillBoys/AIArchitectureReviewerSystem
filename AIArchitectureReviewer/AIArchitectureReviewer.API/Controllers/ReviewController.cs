using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.DTOs.Requests;
using AIArchitectureReviewer.Application.DTOs.Responses;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Domain.Entities;
using AIArchitectureReviewer.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIArchitectureReviewer.API.Controllers
{

    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUMLReviewOrchestrator _orchestrator;
        private readonly AIArchitectureReviewer.Application.Interfaces.Services.IChatService _chatService;
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public ReviewController(
            IUMLReviewOrchestrator orchestrator,
            AIArchitectureReviewer.Application.Interfaces.Services.IChatService chatService,
            ApplicationDbContext context,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _orchestrator = orchestrator;
            _chatService = chatService;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("{sessionId}/chat")]
        public async Task<IActionResult> Chat(System.Guid sessionId, [FromBody] ChatRequestDto request)
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
    }
}
