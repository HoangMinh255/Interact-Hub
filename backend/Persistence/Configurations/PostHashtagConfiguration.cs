namespace InteractHub.Persistence.Configurations;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
using InteractHub.Domain.Entities;
public class PostHashtagConfiguration : IEntityTypeConfiguration<PostHashtag>
{
    public void Configure(EntityTypeBuilder<PostHashtag> builder)
    {
        builder.HasKey(ph => new { ph.PostId, ph.HashtagId });

        builder.HasOne(ph => ph.Post)
               .WithMany(p => p.PostHashtags)
               .HasForeignKey(ph => ph.PostId);

        builder.HasOne(ph => ph.Hashtag)
               .WithMany(h => h.PostHashtags)
               .HasForeignKey(ph => ph.HashtagId);
    }
}