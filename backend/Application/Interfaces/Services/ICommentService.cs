using InteractHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InteractHub.Application.DTOs.Comment;
namespace InteractHub.Application.Interfaces.Services;

public interface ICommentService
{
    Task<IList<Comment>> GetAllComments();
    Task<Comment?> GetCommentById(Guid id);
    Task<IList<Comment>> Get10CommentsFromPostByPostId(Guid PostId, int page = 0);
    Task<IList<Comment>> Get10CommentsByParentCommentIdFromPostByPostId(Guid PostId, Guid parentCommentId, int page = 0);
    Task<Comment> CreateComment(CreateCommentDto comment);
    Task<Comment> UpdateComment(Guid commentId, UpdateCommentDto comment);
    Task<bool> DeleteComment(Guid commentId);
}