using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Domain.Entities;
using InteractHub.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractHub.Persistence.Repositories;

public sealed class StoryRepository : IStoryRepository
{
    private readonly AppDbContext _context;

    public StoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Story?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Set<Story>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Story>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Story>()
            .Where(x => x.UserId == userId && x.IsActive && x.ExpireAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Story>> GetFeedAsync(IEnumerable<string> userIds, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedUserIds = userIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();

        return await _context.Set<Story>()
            .Where(x => normalizedUserIds.Contains(x.UserId) && x.IsActive && x.ExpireAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Story story, CancellationToken cancellationToken = default)
    {
        await _context.Set<Story>().AddAsync(story, cancellationToken);
    }

    public void Update(Story story)
    {
        _context.Set<Story>().Update(story);
    }

    public void Remove(Story story)
    {
        _context.Set<Story>().Remove(story);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
