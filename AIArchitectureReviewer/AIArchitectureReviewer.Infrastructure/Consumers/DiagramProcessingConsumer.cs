using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Domain.Enums;
using DiagramManager.Application.DTOs.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AIArchitectureReviewer.Infrastructure.Consumers;

public class DiagramProcessingConsumer : IConsumer<DiagramProcessingRequestEvent>
{
    private readonly IUMLReviewOrchestrator _orchestrator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DiagramProcessingConsumer> _logger;

    public DiagramProcessingConsumer(
        IUMLReviewOrchestrator orchestrator,
        IPublishEndpoint publishEndpoint,
        ILogger<DiagramProcessingConsumer> logger)
    {
        _orchestrator = orchestrator;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DiagramProcessingRequestEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("AI Service received DiagramVersionId: {DiagramVersionId}, Format: {RawFormat} for processing.", message.DiagramVersionId, message.RawFormat);

        try
        {
            byte[] fileBytes;
            string mimeType;

            string targetPath = message.StorageUrl;
            if (!File.Exists(targetPath))
            {
                var baseDir = Directory.GetCurrentDirectory();
                var relativePath = message.StorageUrl.TrimStart('/', '\\');
                var candidatePath = Path.Combine(baseDir, "wwwroot", relativePath);
                if (File.Exists(candidatePath))
                {
                    targetPath = candidatePath;
                }
            }

            if (message.RawFormat == "mermaid")
            {
                fileBytes = !string.IsNullOrEmpty(message.ContentText)
                    ? System.Text.Encoding.UTF8.GetBytes(message.ContentText)
                    : await File.ReadAllBytesAsync(targetPath, context.CancellationToken);

                mimeType = "text/plain";
            }
            else
            {
                // Read local image file bytes
                fileBytes = await File.ReadAllBytesAsync(targetPath, context.CancellationToken);
                mimeType = message.RawFormat.ToLower() switch
                {
                    "png" => "image/png",
                    "jpg" or "jpeg" => "image/jpeg",
                    _ => "image/png"
                };
            }

            // Gọi hàm ProcessAsync từ IUMLReviewOrchestrator
            var reviewResult = await _orchestrator.ProcessAsync(message.DiagramVersionId, fileBytes, mimeType);

            _logger.LogInformation("Successfully processed DiagramVersionId: {DiagramVersionId}.", message.DiagramVersionId);

            // Bắn Event thông báo hoàn thành về RabbitMQ
            await _publishEndpoint.Publish(new DiagramProcessingCompletedEvent
            {
                DiagramId = message.DiagramId,
                DiagramVersionId = message.DiagramVersionId,
                IsSuccess = true,
                Status = DiagramVersionStatus.Completed,
                ReviewResultSummary = reviewResult.Review?.ToString() ?? "Review completed successfully.",
                ProcessedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process DiagramVersionId: {DiagramVersionId}.", message.DiagramVersionId);

            await _publishEndpoint.Publish(new DiagramProcessingCompletedEvent
            {
                DiagramId = message.DiagramId,
                DiagramVersionId = message.DiagramVersionId,
                IsSuccess = false,
                Status = DiagramVersionStatus.Failed,
                ErrorMessage = ex.Message,
                ProcessedAt = DateTime.UtcNow
            });
        }
    }
}
