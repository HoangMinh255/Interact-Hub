using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Post;
namespace InteractHub.Application.Services;
public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;


    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;  
    }

    public async Task<IList<PostFeedItemDto>> GetAllPosts()
    {
        return await _postRepository.GetAll();
    }
    public async Task<Post?> GetPostById(Guid id)
    {
        return await _postRepository.GetPostById(id);
    }
    public async Task<Post> CreatePost(string userId, CreatePostDto dto)
    {
        //Tạo ID cho bài viết TRƯỚC để dùng làm khóa ngoại cho các bảng khác
        var postId = Guid.NewGuid();

        //Khởi tạo Entity Post
        var post = new Post
        {
            Id = postId,
            UserId = userId,
            Content = dto.Content,
            Visibility = (byte)dto.Visibility,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        //Chuẩn bị danh sách Media (nếu có)
        var medias = new List<PostMedia>();
        if (dto.Media != null && dto.Media.Any())
        {
            int order = 1;
            foreach (var m in dto.Media)
            {
                medias.Add(new PostMedia
                {
                    Id = Guid.NewGuid(),
                    PostId = postId, // Gán thủ công ID bài viết vì không có navigation property
                    MediaUrl = m.MediaUrl,
                    MediaType = (byte)m.MediaType,
                    SortOrder = order++,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
            }
        }

        //Gọi hàm lưu tổng hợp ở Repository (Hàm này dùng Transaction)
        var result = await _postRepository.CreatePostWithDetailsAsync(post, medias, dto.Hashtags ?? new List<string>());
        if(result == null)
        {
            throw new InvalidOperationException("Đã có lỗi xảy ra khi tạo bài viết.");
        }
        return result;
    }
    public async Task<bool> UpdatePostAsync(Guid postId, string userId, UpdatePostDto dto)
    {
        // Chuẩn bị danh sách Media (Mapping từ DTO sang đối tượng Entity cơ bản)
        var parsedMedias = new List<PostMedia>();
        if (dto.Media != null)
        {
            foreach (var m in dto.Media)
            {
                parsedMedias.Add(new PostMedia 
                { 
                    MediaUrl = m.MediaUrl, 
                    MediaType = (byte)m.MediaType 
                });
            }
        }

        // Gọi Repository để xử lý DB
        return await _postRepository.UpdatePostWithDetailsAsync(
            postId, 
            userId, 
            dto.Content, 
            dto.Visibility, 
            parsedMedias, 
            dto.Hashtags
        );
    }
    public async Task<bool> DeletePost(Guid postId, string userId)
    {
        return await _postRepository.DeletePost(postId, userId);
    }
    public async Task<bool> SharePostAsync(string userId, Guid postId, string? comment)
    {
        // Validate post exists
        var post = await _postRepository.GetPostById(postId);
        if (post == null) return false;

        var share = new PostShare
        {
            PostId = postId,
            SharerId = userId,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _postRepository.CreateShareAsync(share);
        return true;
    }

    public async Task<IList<PostShare>> GetSharesByPostIdAsync(Guid postId, int page = 0)
    {
        return await _postRepository.GetSharesByPostIdAsync(postId, page);
    }

    public async Task<IList<Post>> GetSharedPostsByUserId(string userId, int page = 0)
    {
        return await _postRepository.GetSharedPostsByUserIdAsync(userId, page);
    }
    public async Task<IList<PostFeedItemDto>> Get10Posts(int page = 0)
    {
        return await _postRepository.Get10Posts(page);
    }
    public async Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0)
    {
        return await _postRepository.Get10PostsByUserId(userId, page);
    }
}