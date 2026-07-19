using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AIArchitectureReviewer.Application.Interfaces.Orchestrators;
using AIArchitectureReviewer.Application.Interfaces.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace AIArchitectureReviewer.API.Consumers
{
    public class DocumentConsistencyReviewRequestedConsumer : IConsumer<DocumentConsistencyReviewRequestedEvent>
    {
        private readonly IUMLReviewOrchestrator _orchestrator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentConsistencyReviewRequestedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public DocumentConsistencyReviewRequestedConsumer(
            IUMLReviewOrchestrator orchestrator,
            IUnitOfWork unitOfWork,
            ILogger<DocumentConsistencyReviewRequestedConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _orchestrator = orchestrator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DocumentConsistencyReviewRequestedEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation($"Received DocumentConsistencyReviewRequestedEvent for DocumentId: {evt.DocumentId}");

            try
            {
                var diagramsJsonNodes = new List<JsonNode>();

              
                foreach (var versionId in evt.DiagramVersionIds)
                {
                    var reports = await _unitOfWork.AnalysisReports.FindAsync(r => r.DiagramVersionId == versionId);
                    var report = reports.FirstOrDefault();

                    if (report != null && !string.IsNullOrEmpty(report.ParsedDiagram))
                    {
                        var parsedNode = JsonNode.Parse(report.ParsedDiagram);
                        if (parsedNode != null)
                        {
                            diagramsJsonNodes.Add(parsedNode);
                        }
                    }
                }

                if (diagramsJsonNodes.Count < 2)
                {
                    _logger.LogWarning($"Not enough diagrams parsed (found {diagramsJsonNodes.Count}) to perform consistency review for DocumentId: {evt.DocumentId}");
                    return;
                }

                _logger.LogInformation($"Performing consistency review for {diagramsJsonNodes.Count} diagrams...");

              
                var consistencyResult = await _orchestrator.CheckConsistencyAsync(diagramsJsonNodes);

                if (consistencyResult == null)
                {
                    _logger.LogError("Consistency check returned null result.");
                    return;
                }

              
                string reportText = consistencyResult["ConsistencyReport"]?.ToString() ?? "";

                float consistencyScore = 10.0f;
               

                _logger.LogInformation($"Consistency review completed. Publishing Completed Event for DocumentId: {evt.DocumentId}");

           
                await _publishEndpoint.Publish(new DocumentConsistencyReviewCompletedEvent(
                    evt.DocumentId,
                    consistencyScore,
                    reportText
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during consistency review for DocumentId: {evt.DocumentId}");
            }
        }
    }
}