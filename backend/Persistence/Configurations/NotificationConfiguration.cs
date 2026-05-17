namespace InteractHub.Persistence.Configurations;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using InteractHub.Domain.Entities;
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasOne(n => n.Recipient)
               .WithMany(u => u.ReceivedNotifications)
               .HasForeignKey(n => n.RecipientId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Actor)
               .WithMany(u => u.ActedNotifications)
               .HasForeignKey(n => n.ActorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}