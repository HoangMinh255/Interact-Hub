using Microsoft.EntityFrameworkCore;
using InteractHub.Domain.Entities;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Persistence.Data;
using InteractHub.Application.DTOs.Comment;
using Microsoft.AspNetCore.Mvc;
namespace InteractHub.Persistence.Repositories;
public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Comment>> GetAll()
    {
        return await _context.Comments.ToListAsync();
    }

    public async Task<Comment?> GetCommentById(Guid id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<IList<Comment>> Get10CommentsFromPostByPostId(Guid PostId, int page = 0)
    {
        return await _context.Comments.Where(c => c.PostId == PostId && c.IsDeleted == false)
                                      .Include(c => c.User)
                                      .Skip(page * 10)
                                      .Take(10)
                                      .ToListAsync();
    }
    public async Task<IList<Comment>> Get10CommentsByParentCommentIdFromPostByPostId(Guid PostId, Guid parentCommentId, int page = 0)
    {
        return await _context.Comments.Where(c => c.ParentCommentId == parentCommentId && c.PostId == PostId && c.IsDeleted == false)
                                      .Include(c => c.User)
                                      .Skip(page * 10)
                                      .Take(10)
                                      .ToListAsync();
    }

    public async Task<Comment> CreateComment(Comment comment)
    {
        // Tạo Execution Strategy để cho phép EF Core tự động thử lại khi rớt mạng
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await _context.Comments.AddAsync(comment);
            await SaveChanges();
            return comment;
        });
    }

    public async Task<Comment> UpdateComment(Guid commentId, UpdateCommentDto comment)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (existingComment == null)
            {
                return null;
            }
            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = comment.UpdatedAt;
            await SaveChanges();
            return existingComment;
        });
    }

    public async Task<bool> DeleteComment(Guid commentId)
    {
        var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        if(existingComment == null)
        {
            return false;
        }
        existingComment.IsDeleted = true;
        return true;
    }

    public async Task<IList<Comment>> GetCommentsByPostId(Guid postId)
    {
        return await _context.Comments.Where(c => c.PostId == postId && c.IsDeleted == false).ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}