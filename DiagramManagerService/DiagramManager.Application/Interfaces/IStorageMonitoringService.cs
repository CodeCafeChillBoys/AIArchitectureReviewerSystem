using DiagramManager.Application.DTOs;

namespace DiagramManager.Application.Interfaces
{
    public interface IStorageMonitoringService
    {
        StorageSummaryDto GetSummary(IEnumerable<string> trackedStorageUrls, int workspaceLimit = 20);
    }
}
