using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InteractHub.Domain.Entities;

namespace InteractHub.Persistence.Configurations;

public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MediaUrl).IsRequired().HasMaxLength(500);
        
        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_Stories_UserId");
        builder.HasIndex(x => x.ExpireAt).HasDatabaseName("IX_Stories_ExpireAt");

        builder.HasOne(x => x.User)
               .WithMany(u => u.Stories)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}