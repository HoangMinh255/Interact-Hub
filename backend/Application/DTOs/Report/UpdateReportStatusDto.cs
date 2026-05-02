using System.ComponentModel.DataAnnotations;
using InteractHub.Domain.Enums;

namespace InteractHub.Application.DTOs.Report;

public sealed class UpdateReportStatusDto
{
    [Required]
    public ReportStatus Status { get; set; }
}
