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

    public async Task<IList<Post>> GetAllPosts()
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
    public async Task<IActionResult> DeletePost(Guid postId, string userId)
    {
        var isSuccess = await _postRepository.DeletePost(postId, userId);
        if (isSuccess) 
        {
            return new OkResult();
        }
        return new NotFoundResult();
    }
    public async Task<IList<Post>> Get10Posts(int page = 0)
    {
        return await _postRepository.Get10Posts(page);
    }
    public async Task<IList<Post>> Get10PostsByUserId(string userId, int page = 0)
    {
        return await _postRepository.Get10PostsByUserId(userId, page);
    }
}