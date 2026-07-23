namespace DiagramManager.Application.DTOs
{
    public class StorageSummaryDto
    {
        public string StorageType { get; set; } = "LocalFileSystem";
        public long UsedBytes { get; set; }
        public long FileCount { get; set; }
        public long TrackedFileCount { get; set; }
        public long OrphanedFileCount { get; set; }
        public long MissingTrackedFileCount { get; set; }
        public long DiskCapacityBytes { get; set; }
        public long DiskFreeBytes { get; set; }
        public double DiskUsedPercentage { get; set; }
        public DateTime ScannedAtUtc { get; set; }
        public IReadOnlyCollection<StorageCategoryUsageDto> Categories { get; set; } = [];
        public IReadOnlyCollection<WorkspaceStorageUsageDto> Workspaces { get; set; } = [];
    }

    public class StorageCategoryUsageDto
    {
        public string Category { get; set; } = string.Empty;
        public long UsedBytes { get; set; }
        public long FileCount { get; set; }
    }

    public class WorkspaceStorageUsageDto
    {
        public Guid WorkspaceId { get; set; }
        public long UsedBytes { get; set; }
        public long FileCount { get; set; }
    }
}
