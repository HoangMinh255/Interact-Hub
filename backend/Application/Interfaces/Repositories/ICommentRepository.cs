using Microsoft.AspNetCore.Mvc;
using InteractHub.Domain.Entities;
using InteractHub.Application.DTOs.Comment;

namespace InteractHub.Application.Interfaces.Repositories;
public interface ICommentRepository
{
    Task<IList<Comment>> GetAll();
    Task<Comment?> GetCommentById(Guid id);
    Task<IList<Comment>> Get10CommentsFromPostByPostId(Guid PostId, int page = 0);
    Task<IList<Comment>> Get10CommentsByParentCommentIdFromPostByPostId(Guid PostId, Guid parentCommentId, int page = 0);
    Task<Comment> CreateComment(Comment comment);
    Task<Comment> UpdateComment(Guid commentId, UpdateCommentDto comment);
    Task<bool> DeleteComment(Guid commentId);
    Task<IList<Comment>> GetCommentsByPostId(Guid postId);
    Task SaveChanges();
}