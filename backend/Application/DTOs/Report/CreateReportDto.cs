using System.ComponentModel.DataAnnotations;

namespace InteractHub.Application.DTOs.Report;

public sealed class CreateReportDto
{
    [Required]
    public Guid PostId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Reason { get; set; } = string.Empty;
}
