using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using InteractHub.Application.Interfaces.Repositories;
namespace InteractHub.Persistence.Repositories;
public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context)
    {
        _context=context;
    }
    public async Task<IList<Post>> GetAll()
    {
        return await _context.Posts
        .Include(p => p.User)       // Lấy kèm thông tin tác giả
        .Include(p => p.Media)      // Lấy kèm hình ảnh/video của bài viết
        .Include(p => p.Comments)   // Lấy kèm số lượng bình luận (nếu cần)
        .OrderByDescending(p => p.CreatedAt) // Xếp bài mới nhất lên đầu
        .ToListAsync();
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

    public async Task<bool> DeletePost(Guid postId)
    {
        var existingPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == false);
        if (existingPost == null)
        {
            return false;
        }
        existingPost.IsDeleted = true;
        _context.Posts.Update(existingPost);
        await SaveChanges();
        return true;        
    }
    public async Task<IList<Post>> Get10Posts(int page = 0)
    {
        return await _context.Posts .Where(p => p.IsDeleted == false )
                                    .Include(p => p.User)       // Lấy kèm thông tin tác giả
                                    .Include(p => p.Media)      // Lấy kèm hình ảnh/video của bài viết
                                    .Include(p => p.Comments)   // Lấy kèm số lượng bình luận (nếu cần)
                                    .Skip(page*10).Take(10).ToListAsync();
    }
    public async Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0)
    {
        return await _context.Posts.Where(p => p.UserId == userId && p.IsDeleted == false)
                                    .Include(p => p.User)
                                    .Include(p => p.Media)
                                    .Include(p => p.Comments)
                                    .Skip(page*10).Take(10).ToListAsync();
    }
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

}