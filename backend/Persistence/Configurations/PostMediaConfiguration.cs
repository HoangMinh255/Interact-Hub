using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InteractHub.Domain.Entities;

namespace InteractHub.Persistence.Configurations;

public class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MediaUrl).IsRequired().HasMaxLength(500);
        
        builder.HasIndex(x => x.PostId).HasDatabaseName("IX_PostMedia_PostId");

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Media)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}