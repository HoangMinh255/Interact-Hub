using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InteractHub.Domain.Entities;

namespace InteractHub.Persistence.Configurations;

public class PostReportConfiguration : IEntityTypeConfiguration<PostReport>
{
    public void Configure(EntityTypeBuilder<PostReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.PostId).HasDatabaseName("IX_PostReports_PostId");
        builder.HasIndex(x => x.ReporterId).HasDatabaseName("IX_PostReports_ReporterId");

        builder.HasOne(x => x.Post)
               .WithMany(p => p.Reports)
               .HasForeignKey(x => x.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        // Chặn lỗi Multiple Cascade Path từ User
        builder.HasOne(x => x.Reporter)
               .WithMany()
               .HasForeignKey(x => x.ReporterId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ReviewedBy)
               .WithMany()
               .HasForeignKey(x => x.ReviewedById)
               .OnDelete(DeleteBehavior.NoAction);
    }
}