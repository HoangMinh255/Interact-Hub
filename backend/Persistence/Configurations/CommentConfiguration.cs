using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InteractHub.Domain.Entities;

namespace InteractHub.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        // Cấu hình Self-Reference
        builder.HasOne(x => x.ParentComment)
               .WithMany(x => x.Replies)
               .HasForeignKey(x => x.ParentCommentId)
               .OnDelete(DeleteBehavior.NoAction); // BẮT BUỘC dùng NoAction

        // Các cấu hình khác giữ nguyên như cũ
        builder.HasOne(x => x.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Comments)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}