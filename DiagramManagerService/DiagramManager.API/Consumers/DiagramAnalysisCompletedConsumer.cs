using System;
using System.Text.Json;
using System.Threading.Tasks;
using DiagramManager.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace DiagramManager.API.Consumers
{
    public class DiagramAnalysisCompletedConsumer : IConsumer<DiagramAnalysisCompletedEvent>
    {
        private readonly WorkspaceDbContext _dbContext;
        private readonly ILogger<DiagramAnalysisCompletedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IDistributedCache _cache;

        public DiagramAnalysisCompletedConsumer(
            WorkspaceDbContext dbContext,
            ILogger<DiagramAnalysisCompletedConsumer> logger,
            IPublishEndpoint publishEndpoint,
            IDistributedCache cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _cache = cache;
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
                diagramVersion.DiagramType = evt.DiagramType;
                diagramVersion.Status = evt.DiagramType == "Failed" ? "Failed" : "Analyzed";

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Updated DiagramVersion {evt.VersionId} with AI Status: {diagramVersion.Status}, Score: {evt.Score}");

                // Cache AI Review Result in Redis if fileHash exists
                if (diagramVersion.Status == "Analyzed")
                {
                    string versionHashKey = $"diagram:version_hash:{evt.VersionId}";
                    string? fileHash = await _cache.GetStringAsync(versionHashKey);
                    if (!string.IsNullOrEmpty(fileHash))
                    {
                        var cachePayload = new
                        {
                            score = evt.Score,
                            reviewData = evt.ReviewData,
                            diagramType = evt.DiagramType,
                            cachedAt = DateTime.UtcNow
                        };
                        string cacheJson = JsonSerializer.Serialize(cachePayload);
                        await _cache.SetStringAsync($"diagram:hash:{fileHash}", cacheJson, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                        });
                        _logger.LogInformation($"Cached AI Review result to Redis for FileHash: {fileHash}");
                    }
                }
                // 2. Kiểm tra xem diagram này có thuộc một Document nào không
                var diagram = await _dbContext.Diagrams.FirstOrDefaultAsync(d => d.Id == diagramVersion.DiagramId);
                if (diagram != null && diagram.DocumentId.HasValue)
                {
                    var documentId = diagram.DocumentId.Value;
                    _logger.LogInformation($"Diagram belongs to Document: {documentId}. Checking consistency status...");

                    var documentDiagrams = await _dbContext.Diagrams
                        .Where(d => d.DocumentId == documentId)
                        .ToListAsync();
                    var versionIds = new List<Guid>();
                    bool allAnalyzed = true;
                    foreach (var docDiag in documentDiagrams)
                    {
                        var lastestVersion = await _dbContext.DiagramVersions
                            .Where(v => v.DiagramId == docDiag.Id)
                            .OrderByDescending(v => v.UploadedAt)
                            .FirstOrDefaultAsync();
                        if (lastestVersion == null || (lastestVersion.Status != "Analyzed" && lastestVersion.Status != "Failed"))
                        {
                            allAnalyzed = false;
                            _logger.LogInformation($"Diagram {docDiag.Name} (ID: {docDiag.Id}) is not yet processed (status: {(lastestVersion?.Status ?? "none")}).");
                            break;
                        }
                        if (lastestVersion.Status == "Analyzed")
                        {
                            versionIds.Add(lastestVersion.Id);
                        }
                    }
                    // 3. Nếu tất cả các diagram trong document đã phân tích xong -> Gửi sự kiện yêu cầu so sánh đồng nhất
                    if (allAnalyzed && versionIds.Count > 0)
                    {
                        _logger.LogInformation($"All {versionIds.Count} diagrams in Document {documentId} have been analyzed. Triggering consistency review...");
                        await _publishEndpoint.Publish(new DocumentConsistencyReviewRequestedEvent(documentId, versionIds));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing DiagramAnalysisCompletedEvent for VersionId {evt.VersionId}");
            }
        }
    }
}
