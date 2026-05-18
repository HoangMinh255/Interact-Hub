using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<Like?> GetAsync(Guid postId, string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid postId, string userId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<IList<Post>> GetLikedPostsAsync(string userId, CancellationToken cancellationToken = default);

    Task AddAsync(Like like, CancellationToken cancellationToken = default);
    void Remove(Like like);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
