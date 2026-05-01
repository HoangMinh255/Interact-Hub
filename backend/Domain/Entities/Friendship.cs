namespace InteractHub.Domain.Entities;
using InteractHub.Domain.Base;
public class Friendship : BaseEntity
{
    public string RequesterId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public byte Status { get; set; } // 0=Pending, 1=Accepted...
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public bool IsBlocked { get; set; }

    // Navigation
    public ApplicationUser Requester { get; set; } = null!;
    public ApplicationUser Receiver { get; set; } = null!;
}