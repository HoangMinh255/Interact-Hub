using InteractHub.Domain.Base;
namespace InteractHub.Domain.Entities;

public class PostReport : BaseEntity
{
    public Guid PostId { get; set; }
    public string ReporterId { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public byte Status { get; set; } // 0=Pending, 1=Reviewed...
    public string? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public ApplicationUser Reporter { get; set; } = null!;
    public ApplicationUser? ReviewedBy { get; set; }
}