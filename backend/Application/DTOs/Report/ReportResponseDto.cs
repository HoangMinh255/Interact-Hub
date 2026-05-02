using InteractHub.Domain.Enums;

namespace InteractHub.Application.DTOs.Report;

public sealed class ReportResponseDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string ReporterId { get; set; } = string.Empty;
    public string? ReporterName { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public string? ReviewedById { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
