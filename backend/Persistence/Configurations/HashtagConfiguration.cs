using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InteractHub.Domain.Entities;

namespace InteractHub.Persistence.Configurations;

public class HashtagConfiguration : IEntityTypeConfiguration<Hashtag>
{
    public void Configure(EntityTypeBuilder<Hashtag> builder)
    {
        builder.HasKey(x => x.Id);
        
        // Tên Hashtag phải là Duy nhất (Unique)
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
    }
}