using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.DTOs.Comment;
using Microsoft.AspNetCore.Authorization;
using InteractHub.Application.Common;
using System.Security.Claims;
using System.Linq;

namespace InteractHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllComments()
    {
        var comments = await _commentService.GetAllComments();
        return Ok(comments);
    }
    [HttpGet("post/{postId}/page/{page}")]
    public async Task<IActionResult> Get10CommentsByPostId(Guid postId, int page)
    {
        // Lấy danh sách Entity Comment từ Service
        var comments = await _commentService.Get10CommentsFromPostByPostId(postId, page);

        // ÉP KIỂU / BÓC TÁCH DỮ LIỆU ĐỂ CHẶN VÒNG LẶP
        var safeComments = comments.Select(c => new 
        {
            Id = c.Id,
            PostId = c.PostId,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            AuthorName = c.User?.FullName,
            AuthorAvatar = c.User?.AvatarUrl
        });

        // 3. Trả về list dữ liệu đã được làm sạch
        return Ok(safeComments);
    }
    [HttpGet("post/{postId}/parent/{parentCommentId}/page/{page}")]
    public async Task<IActionResult> Get10CommentsByParentCommentIdFromPostByPostId(Guid postId, Guid parentCommentId, int page)
    {
            // Lấy danh sách Entity Comment từ Service
        var comments = await _commentService.Get10CommentsByParentCommentIdFromPostByPostId(postId, parentCommentId, page);

        // ÉP KIỂU / BÓC TÁCH DỮ LIỆU ĐỂ CHẶN VÒNG LẶP
        var safeComments = comments.Select(c => new 
        {
            Id = c.Id,
            PostId = c.PostId,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            AuthorName = c.User?.FullName,
            AuthorAvatar = c.User?.AvatarUrl
        });
        return Ok(safeComments);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var comment = await _commentService.GetCommentById(id);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto commentDto)
    {
        // Lấy UserId từ token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Gán UserId vào DTO
        commentDto.UserId = userId;

        var createdComment = await _commentService.CreateComment(commentDto);
        return Ok(createdComment);
    }

    [HttpPut("{commentId}")]
    [Authorize]
    public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] UpdateCommentDto commentDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var updatedComment = await _commentService.UpdateComment(commentId, commentDto);
        return Ok(updatedComment);
    }

    [HttpDelete("{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var updatedComment = await _commentService.DeleteComment(commentId);
        return Ok(updatedComment);
    }
}
