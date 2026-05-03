using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;

public interface ILikeService
{
    Task<bool> LikeAsync(Guid postId, string userId, CancellationToken cancellationToken = default);
    Task<bool> UnlikeAsync(Guid postId, string userId, CancellationToken cancellationToken = default);
    Task<bool> IsLikedAsync(Guid postId, string userId, CancellationToken cancellationToken = default);
    Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<IList<Post>> GetLikedPostsAsync(string userId, CancellationToken cancellationToken = default);
}
