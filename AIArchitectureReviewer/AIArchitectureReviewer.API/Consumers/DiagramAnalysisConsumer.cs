using System;
using System.Net.Http;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace AIArchitectureReviewer.API.Consumers
{
    public class DiagramAnalysisConsumer : IConsumer<DiagramUploadedEvent>
    {
        private readonly IUMLReviewOrchestrator _orchestrator;
        private readonly ILogger<DiagramAnalysisConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DiagramAnalysisConsumer(IUMLReviewOrchestrator orchestrator, ILogger<DiagramAnalysisConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _orchestrator = orchestrator;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DiagramUploadedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation($"Received DiagramUploadedEvent for DiagramId: {evt.DiagramId}");

            try
            {
                byte[] fileBytes = evt.FileData;
                
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogWarning($"FileData is empty for DiagramId: {evt.DiagramId}");
                    return;
                }
                
                string mimeType = "application/xml";
                var lowerUrl = evt.FileUrl.ToLower();
                if (lowerUrl.EndsWith(".json")) mimeType = "application/json";
                else if (lowerUrl.EndsWith(".png")) mimeType = "image/png";
                else if (lowerUrl.EndsWith(".jpg") || lowerUrl.EndsWith(".jpeg")) mimeType = "image/jpeg";
                else if (lowerUrl.EndsWith(".pdf")) mimeType = "application/pdf";

                _logger.LogInformation($"Received diagram length: {fileBytes.Length}. MimeType: {mimeType}. Passing to AI for review...");

                var result = await _orchestrator.ProcessAsync(evt.VersionId, fileBytes, mimeType, null);

                _logger.LogInformation($"Diagram successfully analyzed. Session ID: {result.SessionId}, Score: {result.Score?.ToJsonString()}");

                // Parse the score
                float scoreValue = 0;
                if (result.Score != null)
                {
                    var scoreNode = result.Score["total_score"] ?? result.Score["TotalScore"] ?? result.Score["score"];
                    if (scoreNode != null && float.TryParse(scoreNode.ToString(), out float parsedScore))
                    {
                        scoreValue = parsedScore;
                    }
                }

                // Publish back to DiagramManagerService
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                await _publishEndpoint.Publish(new DiagramAnalysisCompletedEvent(
                    evt.DiagramId,
                    evt.VersionId,
                    scoreValue,
                    System.Text.Json.JsonSerializer.Serialize(result, jsonOptions)
                ));

                _logger.LogInformation($"Published DiagramAnalysisCompletedEvent for DiagramId: {evt.DiagramId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DiagramUploadedEvent");
            }
        }
    }
}
