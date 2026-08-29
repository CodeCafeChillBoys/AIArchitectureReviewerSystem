using DiagramManager.Application.DTOs.Events;
using DiagramManager.Domain.Entities;
using DiagramManager.Domain.Interfaces;
using DiagramManager.Infrastructure.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiagramManager.Infrastructure.Consumers;

public class DiagramProcessingCompletedConsumer : IConsumer<DiagramProcessingCompletedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<DiagramProcessingCompletedConsumer> _logger;

    public DiagramProcessingCompletedConsumer(
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext,
        ILogger<DiagramProcessingCompletedConsumer> logger)
    {
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DiagramProcessingCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received DiagramProcessingCompletedEvent for DiagramVersionId: {DiagramVersionId}, Status: {Status}, IsSuccess: {IsSuccess}",
            message.DiagramVersionId, message.Status, message.IsSuccess);

        try
        {
            var versionRepo = _unitOfWork.Repository<DiagramVersion>();
            var version = await versionRepo.GetByIdAsync(message.DiagramVersionId, context.CancellationToken);

            if (version == null)
            {
                _logger.LogWarning("DiagramVersion not found with Id: {DiagramVersionId}", message.DiagramVersionId);
                return;
            }

            // Cập nhật trạng thái từ AI Reviewer trả về (Completed / Failed)
            version.Status = message.Status;
            versionRepo.Update(version);

            await _unitOfWork.CompleteAsync(context.CancellationToken);

            _logger.LogInformation("Successfully updated DiagramVersion {DiagramVersionId} status to {Status}",
                message.DiagramVersionId, version.Status);

            // Lấy thông tin Diagram để gửi thông báo chi tiết
            var diagramRepo = _unitOfWork.Repository<Diagram>();
            var diagram = await diagramRepo.GetByIdAsync(version.DiagramId, context.CancellationToken);
            string diagramName = diagram?.Name ?? "Diagram";

            // Gửi Realtime Notification qua SignalR Hub
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                type = "ai_review",
                title = message.IsSuccess ? "AI Analysis Completed" : "AI Analysis Failed",
                message = message.IsSuccess
                    ? $"Architecture review for \"{diagramName}\" (v{version.VersionNumber}) is completed and ready for review."
                    : $"AI Analysis for \"{diagramName}\" failed: {message.ErrorMessage}",
                diagramId = version.DiagramId,
                diagramVersionId = version.Id,
                workspaceId = diagram?.WorkspaceId,
                isSuccess = message.IsSuccess,
                timestamp = DateTime.UtcNow
            }, context.CancellationToken);

            _logger.LogInformation("Successfully sent SignalR notification for DiagramId: {DiagramId}", version.DiagramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for DiagramVersionId: {DiagramVersionId}", message.DiagramVersionId);
            throw;
        }
    }
}
