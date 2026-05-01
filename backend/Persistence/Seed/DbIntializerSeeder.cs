using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace InteractHub.Persistence.Data;

public static class DbInitializer
{
    public static async Task SeedData(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        // 1. Kiểm tra nếu đã có dữ liệu thì không seed nữa
        if (userManager.Users.Any()) return;

        // 2. Tạo User mẫu 1
        var user1 = new ApplicationUser
        {
            UserName = "minh_interact",
            Email = "minh@test.com",
            FullName = "Nguyễn Văn Minh",
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=Minh",
            IsActive = true
        };
        await userManager.CreateAsync(user1, "Password123!");

        // 3. Tạo User mẫu 2
        var user2 = new ApplicationUser
        {
            UserName = "lan_hub",
            Email = "lan@test.com",
            FullName = "Trần Thị Lan",
            AvatarUrl = "https://api.dicebear.com/7.x/avataaars/svg?seed=Lan",
            IsActive = true
        };
        await userManager.CreateAsync(user2, "Password123!");

        // 4. Tạo Post cho User 1
        var post = new Post
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            Content = "Chào mừng mọi người đến với InteractHub! 🚀 #FirstPost",
            Visibility = 0, // Public
            CreatedAt = DateTime.UtcNow,
            Media = new List<PostMedia>
            {
                new PostMedia { 
                    Id = Guid.NewGuid(), 
                    MediaUrl = "https://picsum.photos/800/600", 
                    MediaType = 0, // Image
                    SortOrder = 1 
                }
            }
        };
        context.Posts.Add(post);

        // 5. Tạo Comment cho bài viết từ User 2
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            UserId = user2.Id,
            Content = "Bài viết tuyệt vời quá anh Minh ơi! 😍",
            CreatedAt = DateTime.UtcNow
        };
        context.Comments.Add(comment);

        // 6. Tạo lời mời kết bạn
        var friendship = new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = user1.Id,
            ReceiverId = user2.Id,
            Status = 1, // Accepted
            RequestedAt = DateTime.UtcNow
        };
        context.Friendships.Add(friendship);

        await context.SaveChangesAsync();
    }
}