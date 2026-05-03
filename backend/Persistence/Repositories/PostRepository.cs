using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using InteractHub.Application.DTOs.Post;
using InteractHub.Application.Interfaces.Repositories;
namespace InteractHub.Persistence.Repositories;
public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context)
    {
        _context=context;
    }
    public async Task<IList<PostFeedItemDto>> GetAll()
    {
        return await GetFeedItemsAsync(null);
    }
    public async Task<Post?> GetPostById(Guid id)
    {
        return await _context.Posts .Include(p => p.User)       // Lấy kèm thông tin tác giả
                                    .Include(p => p.Media)      // Lấy kèm hình ảnh/video của bài viết
                                    .Include(p => p.Comments)   // Lấy kèm số lượng bình luận (nếu cần);
                                    .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);                                  
    }                               
    public async Task<Post> CreatePost(Post post)
    {
        _context.Posts.Add(post);
        await SaveChanges();
        return post;
    }

    public async Task<Post> CreatePostWithDetailsAsync(Post post, List<PostMedia> medias, List<string> hashtags)
    {
    // Tạo Execution Strategy để cho phép EF Core tự động thử lại khi rớt mạng
    var strategy = _context.Database.CreateExecutionStrategy();

    return await strategy.ExecuteAsync(async () =>
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Lưu Post
            _context.Posts.Add(post);

            // Lưu Media (nếu có)
            if (medias.Any())
            {
                await _context.PostMedia.AddRangeAsync(medias);
            }

            // Xử lý Hashtags (logic kiểm tra trùng và lưu bảng trung gian)
            if (hashtags.Any())
            {
                foreach (var tag in hashtags.Select(t => t.ToLower().Trim().Trim('#')).Distinct())
                {
                    var tagInDb = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == tag);
                    if (tagInDb == null)
                    {
                        tagInDb = new Hashtag { Id = Guid.NewGuid(), Name = tag, CreatedAt = DateTime.UtcNow };
                        _context.Hashtags.Add(tagInDb);
                    }

                    _context.PostHashtags.Add(new PostHashtag
                    {
                        PostId = post.Id,
                        HashtagId = tagInDb.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return post;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    });
    }

    public async Task<bool> UpdatePostWithDetailsAsync(Guid postId, string userId, string content, int visibility, List<PostMedia>? newMedias, List<string>? newHashtags)
    {
        // 1. Lấy Post lên KÈM THEO các bảng liên quan. Kiểm tra luôn userId.
        var existingPost = await _context.Posts
            .Include(p => p.Media)
            .Include(p => p.PostHashtags)
                .ThenInclude(ph => ph.Hashtag)
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId && !p.IsDeleted);

        if (existingPost == null) return false; // Không tìm thấy hoặc không phải chủ bài viết

        // 2. Cập nhật thông tin bảng chính
        existingPost.Content = content;
        existingPost.Visibility = (byte)visibility;
        existingPost.UpdatedAt = DateTime.UtcNow;

        // 3. Xử lý Media
        if (newMedias != null)
        {
            var incomingUrls = newMedias.Select(m => m.MediaUrl).ToList();
            
            // Xóa ảnh cũ
            var mediaToRemove = existingPost.Media.Where(m => !incomingUrls.Contains(m.MediaUrl)).ToList();
            _context.RemoveRange(mediaToRemove);

            // Thêm ảnh mới
            foreach (var newMedia in newMedias)
            {
                if (!existingPost.Media.Any(m => m.MediaUrl == newMedia.MediaUrl))
                {
                    newMedia.Id = Guid.NewGuid();
                    newMedia.PostId = existingPost.Id;
                    newMedia.CreatedAt = DateTime.UtcNow;
                    existingPost.Media.Add(newMedia);
                }
            }
        }

        // 4. Xử lý Hashtag 
        if (newHashtags != null)
        {
            var incomingTags = newHashtags.Select(t => t.ToLower().Trim().Trim('#')).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();
            var existingTags = existingPost.PostHashtags.Select(ph => ph.Hashtag.Name).ToList();

            // Xóa tag cũ bị gỡ
            var tagsToRemove = existingTags.Except(incomingTags).ToList();
            if (tagsToRemove.Any())
            {
                var relationsToRemove = existingPost.PostHashtags.Where(ph => tagsToRemove.Contains(ph.Hashtag.Name)).ToList();
                _context.PostHashtags.RemoveRange(relationsToRemove);
            }

            // Thêm tag mới
            var tagsToAdd = incomingTags.Except(existingTags).ToList();
            if (tagsToAdd.Any())
            {
                var dbHashtags = await _context.Hashtags.Where(h => tagsToAdd.Contains(h.Name)).ToListAsync();
                foreach (var tagName in tagsToAdd)
                {
                    var tag = dbHashtags.FirstOrDefault(h => h.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Hashtag { Id = Guid.NewGuid(), Name = tagName, CreatedAt = DateTime.UtcNow };
                        _context.Hashtags.Add(tag);
                    }
                    _context.PostHashtags.Add(new PostHashtag { PostId = existingPost.Id, HashtagId = tag.Id, CreatedAt = DateTime.UtcNow });
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePost(Guid postId, string userId)
    {
        var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId && p.IsDeleted == false);
        if (existingPost == null)
        {
            return false;
        }
        existingPost.IsDeleted = true;
        _context.Posts.Update(existingPost);
        await SaveChanges();
        return true;        
    }
    public async Task<IList<PostFeedItemDto>> Get10Posts(int page = 0)
    {
        return await GetFeedItemsAsync(page);
    }
    public async Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0)
    {
        return await _context.Posts.Where(p => p.UserId == userId && p.IsDeleted == false)
                                    .Include(p => p.User)
                                    .Include(p => p.Media)
                                    .Include(p => p.Comments)
                                    .Skip(page*10).Take(10).ToListAsync();
    }
    public async Task<PostShare> CreateShareAsync(PostShare share)
    {
        share.Id = Guid.NewGuid();
        share.CreatedAt = DateTime.UtcNow;
        await _context.PostShares.AddAsync(share);
        await SaveChanges();
        return share;
    }

    public async Task<IList<PostShare>> GetSharesByPostIdAsync(Guid postId, int page = 0)
    {
        return await _context.PostShares
            .Where(s => s.PostId == postId)
            .Include(s => s.Post)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(page * 10)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IList<Post>> GetSharedPostsByUserIdAsync(string userId, int page = 0)
    {
        return await _context.PostShares
            .Where(s => s.SharerId == userId)
            .Include(s => s.Post)
                .ThenInclude(p => p.User)
            .Include(s => s.Post)
                .ThenInclude(p => p.Media)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(page * 10)
            .Take(10)
            .Select(s => s.Post)
            .ToListAsync();
    }
    private async Task<IList<PostFeedItemDto>> GetFeedItemsAsync(int? page)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new { u.Id, u.FullName, u.AvatarUrl })
            .ToDictionaryAsync(u => u.Id, u => new { u.FullName, u.AvatarUrl });

        var posts = await _context.Posts
            .Where(p => !p.IsDeleted)
            .Include(p => p.User)
            .Include(p => p.Media)
            .Include(p => p.Comments)
            .Select(p => new PostFeedItemDto
            {
                Id = p.Id,
                OriginalPostId = p.Id,
                IsShared = false,
                AuthorId = p.UserId,
                AuthorName = p.User.FullName,
                AuthorAvatar = p.User.AvatarUrl,
                Content = p.Content,
                Visibility = p.Visibility,
                CreatedAt = p.CreatedAt,
                MediaUrls = p.Media.Select(m => m.MediaUrl).ToList(),
                CommentCount = p.Comments.Count
            })
            .ToListAsync();

        var shareRows = await _context.PostShares
            .Where(s => !s.IsDeleted && !s.Post.IsDeleted)
            .Include(s => s.Post)
                .ThenInclude(p => p.User)
            .Include(s => s.Post)
                .ThenInclude(p => p.Media)
            .Include(s => s.Post)
                .ThenInclude(p => p.Comments)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var shares = shareRows.Select(s =>
        {
            users.TryGetValue(s.SharerId, out var sharer);
            return new PostFeedItemDto
            {
                Id = s.Id,
                OriginalPostId = s.PostId,
                IsShared = true,
                ShareId = s.Id,
                ShareComment = s.Comment,
                SharedById = s.SharerId,
                SharedByName = sharer?.FullName,
                SharedByAvatar = sharer?.AvatarUrl,
                AuthorId = s.Post.UserId,
                AuthorName = s.Post.User?.FullName,
                AuthorAvatar = s.Post.User?.AvatarUrl,
                Content = s.Post.Content,
                Visibility = s.Post.Visibility,
                CreatedAt = s.CreatedAt,
                MediaUrls = s.Post.Media.Select(m => m.MediaUrl).ToList(),
                CommentCount = s.Post.Comments.Count
            };
        }).ToList();

        var feedItems = posts
            .Concat(shares)
            .OrderByDescending(item => item.CreatedAt)
            .ToList();

        if (page.HasValue)
        {
            feedItems = feedItems
                .Skip(page.Value * 10)
                .Take(10)
                .ToList();
        }

        return feedItems;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

}