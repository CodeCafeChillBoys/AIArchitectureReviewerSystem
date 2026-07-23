using DiagramManager.Application.DTOs;
using DiagramManager.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace DiagramManager.Infrastructure.Services
{
    public class LocalStorageMonitoringService : IStorageMonitoringService
    {
        private readonly string _storageRoot;
        private readonly StringComparer _pathComparer = OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        public LocalStorageMonitoringService(IWebHostEnvironment environment)
        {
            _storageRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "Storage"));
        }

        public StorageSummaryDto GetSummary(IEnumerable<string> trackedStorageUrls, int workspaceLimit = 20)
        {
            var trackedPaths = trackedStorageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(TryResolveTrackedPath)
                .Where(path => path is not null)
                .Select(path => path!)
                .ToHashSet(_pathComparer);

            var files = EnumerateFilesSafely()
                .Select(path => new FileInfo(path))
                .ToList();
            var actualPaths = files.Select(file => file.FullName).ToHashSet(_pathComparer);

            var categories = files
                .GroupBy(file => ResolveCategory(file.FullName))
                .Select(group => new StorageCategoryUsageDto
                {
                    Category = group.Key,
                    FileCount = group.LongCount(),
                    UsedBytes = group.Sum(file => file.Length)
                })
                .OrderByDescending(item => item.UsedBytes)
                .ToList();

            var workspaces = files
                .Select(file => new { File = file, WorkspaceId = ResolveWorkspaceId(file.FullName) })
                .Where(item => item.WorkspaceId.HasValue)
                .GroupBy(item => item.WorkspaceId!.Value)
                .Select(group => new WorkspaceStorageUsageDto
                {
                    WorkspaceId = group.Key,
                    FileCount = group.LongCount(),
                    UsedBytes = group.Sum(item => item.File.Length)
                })
                .OrderByDescending(item => item.UsedBytes)
                .Take(Math.Clamp(workspaceLimit, 1, 100))
                .ToList();

            var drive = new DriveInfo(Path.GetPathRoot(_storageRoot)!);
            var diskUsedPercentage = drive.TotalSize == 0
                ? 0
                : Math.Round((double)(drive.TotalSize - drive.AvailableFreeSpace) / drive.TotalSize * 100, 2);

            return new StorageSummaryDto
            {
                UsedBytes = files.Sum(file => file.Length),
                FileCount = files.Count,
                TrackedFileCount = actualPaths.LongCount(trackedPaths.Contains),
                OrphanedFileCount = actualPaths.LongCount(path => !trackedPaths.Contains(path)),
                MissingTrackedFileCount = trackedPaths.LongCount(path => !actualPaths.Contains(path)),
                DiskCapacityBytes = drive.TotalSize,
                DiskFreeBytes = drive.AvailableFreeSpace,
                DiskUsedPercentage = diskUsedPercentage,
                ScannedAtUtc = DateTime.UtcNow,
                Categories = categories,
                Workspaces = workspaces
            };
        }

        private IEnumerable<string> EnumerateFilesSafely()
        {
            if (!Directory.Exists(_storageRoot))
            {
                return [];
            }

            try
            {
                return Directory.GetFiles(_storageRoot, "*", new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    AttributesToSkip = FileAttributes.ReparsePoint
                });
            }
            catch (IOException)
            {
                return [];
            }
            catch (UnauthorizedAccessException)
            {
                return [];
            }
        }

        private string? TryResolveTrackedPath(string storageUrl)
        {
            try
            {
                var localPath = storageUrl.Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar);
                var fullPath = Path.GetFullPath(Path.Combine(_storageRoot, "..", localPath));
                var relativePath = Path.GetRelativePath(_storageRoot, fullPath);

                return relativePath == ".." || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}")
                    ? null
                    : fullPath;
            }
            catch (Exception exception) when (exception is ArgumentException or IOException or NotSupportedException)
            {
                return null;
            }
        }

        private string ResolveCategory(string fullPath)
        {
            var parts = Path.GetRelativePath(_storageRoot, fullPath)
                .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Any(part => part.Equals("documents", StringComparison.OrdinalIgnoreCase)))
            {
                return "Documents";
            }

            return parts.Any(part => part.Equals("extracted", StringComparison.OrdinalIgnoreCase))
                ? "ExtractedDiagrams"
                : "DiagramUploads";
        }

        private Guid? ResolveWorkspaceId(string fullPath)
        {
            var relativePath = Path.GetRelativePath(_storageRoot, fullPath);
            var firstSegment = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            return Guid.TryParse(firstSegment, out var workspaceId) ? workspaceId : null;
        }
    }
}
