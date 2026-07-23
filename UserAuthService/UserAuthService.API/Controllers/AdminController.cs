using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Application.DTOs;
using UserAuthService.Domain.Entities;
using UserAuthService.Domain.INum;
using UserAuthService.Infrastructure.Persitence.Data;

namespace UserAuthService.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly UserAuthServiceDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AdminController(
        UserAuthServiceDbContext context,
        IWebHostEnvironment environment,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _environment = environment;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        if (!await _context.Database.CanConnectAsync(cancellationToken))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Message = "UserAuth database is unavailable.",
                GeneratedAtUtc = DateTime.UtcNow
            });
        }

        var now = DateTime.UtcNow;
        var since = now.AddHours(-24);
        using var process = Process.GetCurrentProcess();

        var totalUsers = await _context.Users.LongCountAsync(cancellationToken);
        var activeUsers = await _context.Users.LongCountAsync(
            user => user.Isactive != false,
            cancellationToken);
        var inactiveUsers = await _context.Users.LongCountAsync(
            user => user.Isactive == false,
            cancellationToken);
        var adminUsers = await _context.Users.LongCountAsync(
            user => user.Role == (int)UserRole.Admin,
            cancellationToken);
        var newUsersLast24Hours = await _context.Users.LongCountAsync(
            user => user.Createdat >= since,
            cancellationToken);

        var totalNotifications = await _context.NotificationLogs.LongCountAsync(cancellationToken);
        var sentNotifications = await _context.NotificationLogs.LongCountAsync(
            log => log.Status.ToLower() == "sent",
            cancellationToken);
        var failedNotifications = await _context.NotificationLogs.LongCountAsync(
            log => log.Status.ToLower() == "failed",
            cancellationToken);
        var notificationsLast24Hours = await _context.NotificationLogs.LongCountAsync(
            log => log.CreatedAt >= since,
            cancellationToken);
        var dependencies = await GetDependencyHealth(cancellationToken);

        var startedAtUtc = process.StartTime.ToUniversalTime();
        return Ok(new
        {
            GeneratedAtUtc = now,
            Service = new
            {
                Name = "UserAuthService",
                Status = "Healthy",
                Environment = _environment.EnvironmentName,
                StartedAtUtc = startedAtUtc,
                UptimeSeconds = Math.Max(0, (long)(now - startedAtUtc).TotalSeconds),
                WorkingSetBytes = process.WorkingSet64,
                ThreadCount = process.Threads.Count,
                ProcessorCount = Environment.ProcessorCount
            },
            Database = new { Status = "Healthy" },
            Dependencies = dependencies,
            Users = new
            {
                Total = totalUsers,
                Active = activeUsers,
                Inactive = inactiveUsers,
                Admins = adminUsers,
                NewLast24Hours = newUsersLast24Hours
            },
            Notifications = new
            {
                Total = totalNotifications,
                Sent = sentNotifications,
                Failed = failedNotifications,
                CreatedLast24Hours = notificationsLast24Hours
            }
        });
    }

    [HttpGet("notification-logs")]
    public async Task<IActionResult> GetNotificationLogs(
        [FromQuery] string? channel,
        [FromQuery] string? status,
        [FromQuery] string? recipient,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return BadRequest(new
            {
                Message = "page must be at least 1 and pageSize must be between 1 and 100."
            });
        }

        if (fromUtc.HasValue && toUtc.HasValue && fromUtc > toUtc)
        {
            return BadRequest(new
            {
                Message = "fromUtc must be earlier than or equal to toUtc."
            });
        }

        var query = ApplyNotificationFilters(
            _context.NotificationLogs.AsNoTracking(),
            channel,
            status,
            recipient,
            fromUtc,
            toUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(log => log.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [HttpGet("notification-logs/summary")]
    public async Task<IActionResult> GetNotificationLogSummary(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        if (fromUtc.HasValue && toUtc.HasValue && fromUtc > toUtc)
        {
            return BadRequest(new
            {
                Message = "fromUtc must be earlier than or equal to toUtc."
            });
        }

        var query = ApplyNotificationFilters(
            _context.NotificationLogs.AsNoTracking(),
            channel: null,
            status: null,
            recipient: null,
            fromUtc,
            toUtc);

        var total = await query.CountAsync(cancellationToken);
        var byStatus = await query
            .GroupBy(log => log.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .OrderByDescending(item => item.Count)
            .ToListAsync(cancellationToken);
        var byChannel = await query
            .GroupBy(log => log.Channel)
            .Select(group => new { Channel = group.Key, Count = group.Count() })
            .OrderByDescending(item => item.Count)
            .ToListAsync(cancellationToken);

        return Ok(new { Total = total, ByStatus = byStatus, ByChannel = byChannel });
    }

    [HttpGet("notification-logs/{id:guid}")]
    public async Task<IActionResult> GetNotificationLog(
        Guid id,
        CancellationToken cancellationToken)
    {
        var log = await _context.NotificationLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return log is null ? NotFound() : Ok(log);
    }

    [HttpPost("notification-logs")]
    public async Task<IActionResult> CreateNotificationLog(
        [FromBody] CreateNotificationLogRequest request,
        CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Channel = request.Channel.Trim(),
            Recipient = request.Recipient.Trim(),
            Subject = request.Subject?.Trim(),
            Status = request.Status.Trim(),
            Provider = request.Provider?.Trim(),
            ProviderMessageId = request.ProviderMessageId?.Trim(),
            ErrorMessage = request.ErrorMessage?.Trim(),
            CorrelationId = request.CorrelationId?.Trim(),
            CreatedAt = DateTime.UtcNow,
            SentAt = request.SentAt?.ToUniversalTime()
        };

        _context.NotificationLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetNotificationLog), new { id = log.Id }, log);
    }

    private async Task<IReadOnlyCollection<object>> GetDependencyHealth(
        CancellationToken cancellationToken)
    {
        var endpoints = _configuration
            .GetSection("AdminMonitoring:DependencyHealthUrls")
            .GetChildren()
            .Where(item => !string.IsNullOrWhiteSpace(item.Value))
            .Select(item => new { Name = item.Key, Url = item.Value! })
            .ToList();

        var client = _httpClientFactory.CreateClient("AdminHealthChecks");
        var checks = endpoints.Select(async endpoint =>
        {
            try
            {
                using var response = await client.GetAsync(endpoint.Url, cancellationToken);
                return (object)new
                {
                    endpoint.Name,
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy"
                };
            }
            catch (Exception)
            {
                return new { endpoint.Name, Status = "Unavailable" };
            }
        });

        return await Task.WhenAll(checks);
    }

    private static IQueryable<NotificationLog> ApplyNotificationFilters(
        IQueryable<NotificationLog> query,
        string? channel,
        string? status,
        string? recipient,
        DateTime? fromUtc,
        DateTime? toUtc)
    {
        if (!string.IsNullOrWhiteSpace(channel))
        {
            var normalizedChannel = channel.Trim().ToLower();
            query = query.Where(log => log.Channel.ToLower() == normalizedChannel);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToLower();
            query = query.Where(log => log.Status.ToLower() == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(recipient))
        {
            var normalizedRecipient = recipient.Trim().ToLower();
            query = query.Where(log => log.Recipient.ToLower().Contains(normalizedRecipient));
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(log => log.CreatedAt >= fromUtc.Value.ToUniversalTime());
        }

        if (toUtc.HasValue)
        {
            query = query.Where(log => log.CreatedAt <= toUtc.Value.ToUniversalTime());
        }

        return query;
    }
}
