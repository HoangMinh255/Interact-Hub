using System.ComponentModel.DataAnnotations;
using InteractHub.Domain.Enums;

namespace InteractHub.Application.DTOs.Report;

public sealed class ReportQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public ReportStatus? Status { get; set; }
}
