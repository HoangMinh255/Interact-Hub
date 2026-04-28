using InteractHub.Application.Interfaces.Services;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.DTOs.Comment;
using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InteractHub.Application.Services;
public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    public async Task<IList<Comment>> GetAllComments()
    {
        return await _commentRepository.GetAll();
    }
    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await _commentRepository.GetCommentById(id);
    }
    public async Task<IList<Comment>> Get10CommentsFromPostByPostId(Guid PostId, int page = 0)
    {
        return await _commentRepository.Get10CommentsFromPostByPostId(PostId, page);
    }
    public async Task<IList<Comment>> Get10CommentsByParentCommentIdFromPostByPostId(Guid PostId, Guid parentCommentId, int page = 0)
    {
        return await _commentRepository.Get10CommentsByParentCommentIdFromPostByPostId(PostId, parentCommentId, page);
    }
    public async Task<Comment> CreateComment(CreateCommentDto comment)
    {
        var commentEntity = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = DateTime.UtcNow
        };
        return await _commentRepository.CreateComment(commentEntity);
    }
    public async Task<Comment> UpdateComment(Guid commentId, UpdateCommentDto comment)
    {
        return await _commentRepository.UpdateComment(commentId, comment);
    }
    public async Task<bool> DeleteComment(Guid commentId)
    {
        return await _commentRepository.DeleteComment(commentId);
    }
    public async Task<IList<Comment>> GetCommentsByPostId(Guid postId)
    {
        return await _commentRepository.GetCommentsByPostId(postId);
    }
}