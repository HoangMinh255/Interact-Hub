namespace InteractHub.Persistence.Configurations;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using InteractHub.Domain.Entities;
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("getutcdate()");
        
        // Index đề xuất trong bản thiết kế
        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_Posts_UserId");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("IX_Posts_CreatedAt");

        builder.HasOne(x => x.User)
               .WithMany(u => u.Posts)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}