using System;
using System.Threading.Tasks;
using DiagramManager.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace DiagramManager.API.Consumers
{
    public class DocumentConsistencyReviewCompletedConsumer : IConsumer<DocumentConsistencyReviewCompletedEvent>
    {
        private readonly WorkspaceDbContext _dbContext;
        private readonly ILogger<DocumentConsistencyReviewCompletedConsumer> _logger;

        public DocumentConsistencyReviewCompletedConsumer(WorkspaceDbContext dbContext, ILogger<DocumentConsistencyReviewCompletedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DocumentConsistencyReviewCompletedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation($"Received DocumentConsistencyReviewCompletedEvent for DocumentId: {evt.DocumentId}");

            try
            {
                var document = await _dbContext.Documents.FirstOrDefaultAsync(d => d.Id == evt.DocumentId);

                if (document == null)
                {
                    _logger.LogWarning($"Document {evt.DocumentId} not found in database.");
                    return;
                }

                document.ConsistencyScore = evt.ConsistencyScore;
                document.ConsistencyReview = evt.ConsistencyReviewData;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated Document {evt.DocumentId} with Consistency Score: {evt.ConsistencyScore}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating Document {evt.DocumentId} with consistency results");
            }
        }
    }
}
