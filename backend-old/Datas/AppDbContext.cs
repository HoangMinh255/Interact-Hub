using System.Xml.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(){}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<PostHashtags> PostHashtags { get; set; }
    public DbSet<Notifications> Notifications { get; set; }
    public DbSet<Likes> Likes { get; set; }
    public DbSet<Hashtags> Hashtags { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
        // Keyless entity configuration
        modelBuilder.Entity<PostHashtags>().HasNoKey();
        // Có thể thêm các cấu hình khác nếu cần
    }

}