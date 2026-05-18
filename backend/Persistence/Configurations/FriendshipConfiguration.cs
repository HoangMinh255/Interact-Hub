namespace InteractHub.Persistence.Configurations;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using InteractHub.Domain.Entities;
public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(x => x.Id);
        
        // Ràng buộc Unique cặp (Requester, Receiver)
        builder.HasIndex(x => new { x.RequesterId, x.ReceiverId }).IsUnique();

        builder.HasOne(x => x.Requester)
               .WithMany(u => u.SentFriendRequests)
               .HasForeignKey(x => x.RequesterId)
               .OnDelete(DeleteBehavior.Restrict); // Tránh Multiple Cascade Paths

        builder.HasOne(x => x.Receiver)
               .WithMany(u => u.ReceivedFriendRequests)
               .HasForeignKey(x => x.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}