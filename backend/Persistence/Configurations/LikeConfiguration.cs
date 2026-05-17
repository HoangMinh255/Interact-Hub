using InteractHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InteractHub.Persistence.Configurations;

public sealed class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(x => new { x.PostId, x.UserId });

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.HasIndex(x => x.PostId)
            .HasDatabaseName("IX_Likes_PostId");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Likes_UserId");

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
