using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Api.Extensions;

public static class SeedingExtensions
{
    public static async Task SeedIdentityAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();

        // Chỉ chạy nếu Database chưa có User nào
        if (!await userManager.Users.AnyAsync())
        {
            // ==========================================
            // 1. TẠO 3 NGƯỜI DÙNG THỰC TẾ
            // ==========================================
            var user1 = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "tuankiet_it", Email = "kiet@test.com", FullName = "Nguyễn Tuấn Kiệt", AvatarUrl = "https://i.pravatar.cc/150?u=kiet", Bio = "Lập trình viên | Đam mê công nghệ & Cafe ☕", EmailConfirmed = true, IsActive = true };
            var user2 = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "minhanh_99", Email = "anh@test.com", FullName = "Trần Minh Anh", AvatarUrl = "https://i.pravatar.cc/150?u=anh", Bio = "Thích xê dịch và chụp ảnh 📸", EmailConfirmed = true, IsActive = true };
            var user3 = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "hoanglong_dev", Email = "long@test.com", FullName = "Lê Hoàng Long", AvatarUrl = "https://i.pravatar.cc/150?u=long", Bio = "Âm nhạc là cuộc sống 🎸", EmailConfirmed = true, IsActive = true };

            // Mật khẩu chung cho cả 3 user là: Pass123$
            await userManager.CreateAsync(user1, "Pass123$");
            await userManager.CreateAsync(user2, "Pass123$");
            await userManager.CreateAsync(user3, "Pass123$");

            // ==========================================
            // 2. TẠO BẠN BÈ (Friendships)
            // ==========================================
            context.Friendships.AddRange(
                new Friendship { Id = Guid.NewGuid(), RequesterId = user1.Id, ReceiverId = user2.Id, Status = 1, RequestedAt = DateTime.UtcNow.AddDays(-5) }, // Kiệt và Anh là bạn
                new Friendship { Id = Guid.NewGuid(), RequesterId = user2.Id, ReceiverId = user3.Id, Status = 1, RequestedAt = DateTime.UtcNow.AddDays(-2) }  // Anh và Long là bạn
            );

            // ==========================================
            // 3. TẠO BÀI VIẾT (Posts) KÈM HÌNH ẢNH THẬT
            // ==========================================
            var post1Id = Guid.NewGuid();
            var post2Id = Guid.NewGuid();
            var post3Id = Guid.NewGuid();

            context.Posts.AddRange(
                // Bài của Minh Anh (Đăng 2 tiếng trước)
                new Post { Id = post1Id, UserId = user2.Id, Content = "Bình minh trên biển Vũng Tàu hôm nay tuyệt quá mọi người ơi! Đã sạc đầy năng lượng cho tuần mới 🌅🌊 #travel #vungtau", Visibility = 0, CreatedAt = DateTime.UtcNow.AddHours(-2) },
                // Bài của Tuấn Kiệt (Đăng hôm qua)
                new Post { Id = post2Id, UserId = user1.Id, Content = "Vừa setup xong góc làm việc mới. Bàn phím cơ gõ sướng thật sự! Sẵn sàng chạy deadline cuối năm ⌨️💻 #setup #workspace", Visibility = 0, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                // Bài của Hoàng Long (Đăng 5 phút trước)
                new Post { Id = post3Id, UserId = user3.Id, Content = "Sáng nay nghe được một bản nhạc Indie hay quá, recommend mọi người cùng nghe nha 🎧✨", Visibility = 0, CreatedAt = DateTime.UtcNow.AddMinutes(-5) }
            );

            context.PostMedia.AddRange(
                new PostMedia { Id = Guid.NewGuid(), PostId = post1Id, MediaUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e", MediaType = 0, SortOrder = 1 }, // Ảnh biển
                new PostMedia { Id = Guid.NewGuid(), PostId = post2Id, MediaUrl = "https://images.unsplash.com/photo-1498050108023-c5249f4df085", MediaType = 0, SortOrder = 1 }  // Ảnh bàn làm việc
            );

            // ==========================================
            // 4. TẠO TƯƠNG TÁC (Comments & Likes)
            // ==========================================
            var cmt1Id = Guid.NewGuid();
            
            context.Comments.AddRange(
                // Kiệt và Long comment bài của Anh
                new Comment { Id = cmt1Id, PostId = post1Id, UserId = user3.Id, Content = "Đẹp quá Anh ơi! Đi mấy ngày vậy?", CreatedAt = DateTime.UtcNow.AddHours(-1) },
                new Comment { Id = Guid.NewGuid(), PostId = post1Id, UserId = user1.Id, Content = "Chụp bằng máy gì mà nét thế em?", CreatedAt = DateTime.UtcNow.AddMinutes(-45) },
                // Anh reply Long
                new Comment { Id = Guid.NewGuid(), PostId = post1Id, UserId = user2.Id, ParentCommentId = cmt1Id, Content = "Mình đi 3 ngày 2 đêm á Long. Không khí thích lắm!", CreatedAt = DateTime.UtcNow.AddMinutes(-30) },
                // Anh comment bài của Kiệt
                new Comment { Id = Guid.NewGuid(), PostId = post2Id, UserId = user2.Id, Content = "Bàn phím hãng nào thế Kiệt? Nhìn ngầu quá!", CreatedAt = DateTime.UtcNow.AddDays(-1).AddHours(2) }
            );

            context.Likes.AddRange(
                new Like { PostId = post1Id, UserId = user1.Id },
                new Like { PostId = post1Id, UserId = user3.Id },
                new Like { PostId = post2Id, UserId = user2.Id },
                new Like { PostId = post3Id, UserId = user1.Id }
            );
            // ==========================================
            // 5. TẠO HASHTAG VÀ LIÊN KẾT (Hashtags & PostHashtags)
            // ==========================================
            // Lấy từ content của các bài viết ở trên (#travel, #vungtau, #setup, #workspace)
            var tagTravelId = Guid.NewGuid();
            var tagVungTauId = Guid.NewGuid();
            var tagSetupId = Guid.NewGuid();
            var tagWorkspaceId = Guid.NewGuid();

            context.Hashtags.AddRange(
                new Hashtag { Id = tagTravelId, Name = "travel", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Hashtag { Id = tagVungTauId, Name = "vungtau", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Hashtag { Id = tagSetupId, Name = "setup", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Hashtag { Id = tagWorkspaceId, Name = "workspace", CreatedAt = DateTime.UtcNow.AddDays(-2) }
            );

            context.PostHashtags.AddRange(
                // Gắn tag cho bài 1 của Minh Anh
                new PostHashtag { PostId = post1Id, HashtagId = tagTravelId, CreatedAt = DateTime.UtcNow.AddHours(-2) },
                new PostHashtag { PostId = post1Id, HashtagId = tagVungTauId, CreatedAt = DateTime.UtcNow.AddHours(-2) },
                // Gắn tag cho bài 2 của Tuấn Kiệt
                new PostHashtag { PostId = post2Id, HashtagId = tagSetupId, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new PostHashtag { PostId = post2Id, HashtagId = tagWorkspaceId, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            );

            // ==========================================
            // 6. TẠO THÔNG BÁO (Notifications)
            // ==========================================
            context.Notifications.AddRange(
                // 1. Minh Anh (user2) nhận được thông báo do Tuấn Kiệt (user1) like bài viết
                new Notification 
                { 
                    Id = Guid.NewGuid(), 
                    RecipientId = user2.Id,     // Người nhận là Minh Anh
                    ActorId = user1.Id,         // Người thực hiện là Tuấn Kiệt
                    Content = "Tuấn Kiệt đã thích bài viết của bạn.", 
                    Type = 0,                   // Giả sử 0 là thông báo Like
                    RelatedEntityId = post1Id.ToString(), // ID của bài viết được Like
                    RelatedEntityType = "Post",           // Loại đối tượng là Bài viết
                    IsRead = false, 
                    IsDeleted = false, 
                    CreatedAt = DateTime.UtcNow.AddHours(-1) 
                },

                // 2. Minh Anh (user2) nhận thông báo do Hoàng Long (user3) comment
                new Notification 
                { 
                    Id = Guid.NewGuid(), 
                    RecipientId = user2.Id,     // Người nhận là Minh Anh
                    ActorId = user3.Id,         // Người thực hiện là Hoàng Long
                    Content = "Hoàng Long đã bình luận về bài viết của bạn: 'Đẹp quá Anh ơi...'", 
                    Type = 1,                   // Giả sử 1 là thông báo Comment
                    RelatedEntityId = post1Id.ToString(), // ID của bài viết được comment
                    RelatedEntityType = "Post",           
                    IsRead = true, 
                    IsDeleted = false, 
                    CreatedAt = DateTime.UtcNow.AddMinutes(-50) 
                },

                // 3. Tuấn Kiệt (user1) nhận được thông báo do Minh Anh (user2) đồng ý kết bạn
                new Notification 
                { 
                    Id = Guid.NewGuid(), 
                    RecipientId = user1.Id,     // Người nhận là Tuấn Kiệt
                    ActorId = user2.Id,         // Người thực hiện là Minh Anh
                    Content = "Minh Anh đã chấp nhận lời mời kết bạn của bạn.", 
                    Type = 3,                   // Giả sử 3 là thông báo FriendAccept
                    RelatedEntityId = user2.Id, // Trỏ về ID của Minh Anh để click vào xem profile
                    RelatedEntityType = "User", // Loại đối tượng là User
                    IsRead = true, 
                    IsDeleted = false, 
                    CreatedAt = DateTime.UtcNow.AddDays(-5) 
                }
            );

            // ==========================================
            // 7. LƯU TẤT CẢ VÀO DATABASE
            // ==========================================
            await context.SaveChangesAsync();
        }
    }
}