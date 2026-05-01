using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Post;
using Microsoft.AspNetCore.Authorization;
using InteractHub.Application.Common;
using System.Security.Claims;
using System.Linq;

namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllPosts();

        // Map sang Anonymous Object hoặc DTO để cắt đứt vòng lặp JSON
        var result = posts.Select(p => new 
        {
            Id = p.Id,
            Content = p.Content,
            Visibility = p.Visibility,
            CreatedAt = p.CreatedAt,
            // Lấy những chuỗi cần thiết từ User
            AuthorName = p.User?.FullName, 
            AuthorAvatar = p.User?.AvatarUrl,
            // Lấy URL của danh sách ảnh
            MediaUrls = p.Media?.Select(m => m.MediaUrl).ToList(),
            // Đếm số lượng comment 
            CommentCount = p.Comments?.Count ?? 0 
        });

        return Ok(result);
    }

    [HttpGet("page/{page}")]
    public async Task<IActionResult> Get10Posts(int page = 0)
    {
        var posts = await _postService.Get10Posts(page);

        var result = posts.Select(p => new 
        {
            Id = p.Id,
            Content = p.Content,
            Visibility = p.Visibility,
            CreatedAt = p.CreatedAt,
            // Lấy những chuỗi cần thiết từ User
            AuthorName = p.User?.FullName, 
            AuthorAvatar = p.User?.AvatarUrl,
            // Lấy URL của danh sách ảnh
            MediaUrls = p.Media?.Select(m => m.MediaUrl).ToList(),
            // Đếm số lượng comment 
            CommentCount = p.Comments?.Count ?? 0 
        });

        return Ok(result);
    }

    [HttpGet("user/{userId}/page/{page}")]
    public async Task<IActionResult> Get10PostsByUserId(string userId, int page)
    {
        var posts = await _postService.Get10PostsByUserId(userId, page);

        var result = posts.Select(p => new 
        {
            Id = p.Id,
            Content = p.Content,
            Visibility = p.Visibility,
            CreatedAt = p.CreatedAt,
            // Lấy những chuỗi cần thiết từ User
            AuthorName = p.User?.FullName, 
            AuthorAvatar = p.User?.AvatarUrl,
            // Lấy URL của danh sách ảnh
            MediaUrls = p.Media?.Select(m => m.MediaUrl).ToList(),
            // Đếm số lượng comment 
            CommentCount = p.Comments?.Count ?? 0 
        });

        return Ok(result);
    }

    // [HttpGet("id/{id}")]
    // public async Task<IActionResult> GetPostById(Guid id)
    // {
    //     var post = await _postService.GetPostById(id);
    //     if (post == null)
    //     {
    //         return NotFound();
    //     }

    //     var result = post.Select(p => new 
    //     {
    //         Id = p.Id,
    //         Content = p.Content,
    //         Visibility = p.Visibility,
    //         CreatedAt = p.CreatedAt,
    //         // Lấy những chuỗi cần thiết từ User
    //         AuthorName = p.User?.FullName, 
    //         AuthorAvatar = p.User?.AvatarUrl,
    //         // Lấy URL của danh sách ảnh
    //         MediaUrls = p.Media?.Select(m => m.MediaUrl).ToList(),
    //         // Đếm số lượng comment 
    //         CommentCount = p.Comments?.Count ?? 0 
    //     });

    //     return Ok(result);
    // }
    

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Không thể xác thực danh tính người dùng." });
            }

            if(string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest(new { message = "Bài viết phải có nội dung hoặc ít nhất một media." });
            }

            var createdPost = await _postService.CreatePost(userId, dto);
            return Ok(new 
            { 
                message = "Đăng bài viết thành công!", 
                postId = createdPost.Id 
            });
        }
        catch(Exception ex)
        {
            // Log lỗi ở đây nếu cần
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo bài viết.", error = ex.Message });
        }
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostDto dto)
    {
        try
        {
            // Lấy ID của người dùng đang gọi API từ JWT Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Validate DTO cơ bản
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return BadRequest(new { message = "Nội dung bài viết không được để trống." });
            }

            // Gọi Service xử lý
            var isSuccess = await _postService.UpdatePostAsync(id, userId, dto);

            if (!isSuccess)
            {
                // Trả về NotFound nếu bài viết không tồn tại, HOẶC người sửa không phải là chủ bài viết
                return NotFound(new { message = "Không tìm thấy bài viết hoặc bạn không có quyền chỉnh sửa!" });
            }

            return Ok(new { message = "Cập nhật bài viết thành công!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống.", error = ex.Message });
        }
    }


    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        try{
            var result = await _postService.DeletePost(postId);
            return Ok(new { message = "Xóa bài viết thành công!" });
        }
        catch(Exception e)
        {
            return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống: ", error = e.Message });
        }
        
        
    }


}