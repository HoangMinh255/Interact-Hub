namespace InteractHub.Persistence.Configurations;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using InteractHub.Domain.Entities;
public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        // Khóa chính kép (PostId, UserId) chống like trùng
        builder.HasKey(l => new { l.PostId, l.UserId });

        builder.HasOne(l => l.Post)
               .WithMany(p => p.Likes)
               .HasForeignKey(l => l.PostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}