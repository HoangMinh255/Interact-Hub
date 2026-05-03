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

    public async Task<IList<Post>> GetLikedPostsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var likes = await _context.Likes
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Include(x => x.Post)
                .ThenInclude(p => p.User)
            .Include(x => x.Post)
                .ThenInclude(p => p.Media)
            .Include(x => x.Post)
                .ThenInclude(p => p.Comments)
            .Include(x => x.Post)
                .ThenInclude(p => p.Likes)
            .Where(x => x.Post != null && !x.Post.IsDeleted)
            .ToListAsync(cancellationToken);

        return likes
            .Select(x => x.Post)
            .Where(post => post is not null)
            .Select(post => post!)
            .ToList();
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
