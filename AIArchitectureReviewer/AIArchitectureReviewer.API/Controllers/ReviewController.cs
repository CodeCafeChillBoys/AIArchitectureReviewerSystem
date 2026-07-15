using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;

using AIArchitectureReviewer.API.DTOs;

namespace AIArchitectureReviewer.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUMLReviewOrchestrator _orchestrator;
        private readonly AIArchitectureReviewer.Application.Interfaces.Services.IChatService _chatService;

        public ReviewController(IUMLReviewOrchestrator orchestrator, AIArchitectureReviewer.Application.Interfaces.Services.IChatService chatService)
        {
            _orchestrator = orchestrator;
            _chatService = chatService;
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
    }
}
