using System;
using System.Threading.Tasks;
using DiagramManager.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace DiagramManager.API.Consumers
{
    public class DiagramAnalysisCompletedConsumer : IConsumer<DiagramAnalysisCompletedEvent>
    {
        private readonly WorkspaceDbContext _dbContext;
        private readonly ILogger<DiagramAnalysisCompletedConsumer> _logger;

        public DiagramAnalysisCompletedConsumer(WorkspaceDbContext dbContext, ILogger<DiagramAnalysisCompletedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DiagramAnalysisCompletedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation($"Received DiagramAnalysisCompletedEvent for VersionId: {evt.VersionId}");

            try
            {
                var diagramVersion = await _dbContext.DiagramVersions.FirstOrDefaultAsync(v => v.Id == evt.VersionId);
                
                if (diagramVersion == null)
                {
                    _logger.LogWarning($"DiagramVersion {evt.VersionId} not found in database.");
                    return;
                }

                diagramVersion.AiScore = evt.Score;
                diagramVersion.AiReview = evt.ReviewData;
                diagramVersion.Status = "Analyzed";

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Updated DiagramVersion {evt.VersionId} with AI Score: {evt.Score}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing DiagramAnalysisCompletedEvent for VersionId {evt.VersionId}");
            }
        }
    }
}
