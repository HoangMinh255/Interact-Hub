using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Persistence.Repositories;

public sealed class LikeRepository : ILikeRepository
{
    private readonly AppDbContext _context;

    public LikeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Like?> GetAsync(Guid postId, string userId, CancellationToken cancellationToken = default)
    {
        return _context.Likes
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid postId, string userId, CancellationToken cancellationToken = default)
    {
        return _context.Likes
            .AnyAsync(x => x.PostId == postId && x.UserId == userId, cancellationToken);
    }

    public Task<int> CountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return _context.Likes
            .CountAsync(x => x.PostId == postId, cancellationToken);
    }

    public async Task AddAsync(Like like, CancellationToken cancellationToken = default)
    {
        await _context.Likes.AddAsync(like, cancellationToken);
    }

    public void Remove(Like like)
    {
        _context.Likes.Remove(like);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
