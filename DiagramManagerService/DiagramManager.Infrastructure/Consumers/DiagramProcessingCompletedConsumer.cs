using DiagramManager.Application.DTOs.Events;
using DiagramManager.Domain.Entities;
using DiagramManager.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiagramManager.Infrastructure.Consumers;

public class DiagramProcessingCompletedConsumer : IConsumer<DiagramProcessingCompletedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DiagramProcessingCompletedConsumer> _logger;

    public DiagramProcessingCompletedConsumer(
        IUnitOfWork unitOfWork,
        ILogger<DiagramProcessingCompletedConsumer> logger)
    {
        _unitOfWork = unitOfWork;
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for DiagramVersionId: {DiagramVersionId}", message.DiagramVersionId);
            throw;
        }
    }
}
