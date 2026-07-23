using System.ComponentModel.DataAnnotations;

namespace UserAuthService.Application.DTOs;

public class CreateNotificationLogRequest
{
    [Required, MaxLength(32)]
    public string Channel { get; set; } = string.Empty;

    [Required, MaxLength(320)]
    public string Recipient { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Subject { get; set; }

    [Required, MaxLength(32)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Provider { get; set; }

    [MaxLength(255)]
    public string? ProviderMessageId { get; set; }

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    [MaxLength(100)]
    public string? CorrelationId { get; set; }

    public DateTime? SentAt { get; set; }
}
