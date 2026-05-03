using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using InteractHub.Domain.Enums;
using InteractHub.Application.Common.Exceptions;

namespace InteractHub.Application.Services;

public sealed class LikeService : ILikeService
{
    private readonly ILikeRepository _likeRepository;
    private readonly IPostRepository _postRepository;
    private readonly INotificationRepository _notificationRepository;

    public LikeService(
        ILikeRepository likeRepository,
        IPostRepository postRepository,
        INotificationRepository notificationRepository)
    {
        _likeRepository = likeRepository;
        _postRepository = postRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<bool> LikeAsync(Guid postId, string userId, CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetPostById(postId);
        if (post is null)
            throw new NotFoundException("Post not found.");

        var existingLike = await _likeRepository.GetAsync(postId, userId, cancellationToken);
        if (existingLike is not null)
            return true;

        var like = new Like
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _likeRepository.AddAsync(like, cancellationToken);
        await _likeRepository.SaveChangesAsync(cancellationToken);

        if (!string.Equals(post.UserId, userId, StringComparison.Ordinal))
        {
            var notification = new Notification
            {
                RecipientId = post.UserId,
                ActorId = userId,
                Type = (byte)NotificationType.PostLiked,
                Content = "A user liked your post.",
                RelatedEntityType = nameof(Post),
                RelatedEntityId = postId.ToString(),
                IsRead = false
            };

            await _notificationRepository.CreateNotification(notification);
        }

        return true;
    }

    public async Task<bool> UnlikeAsync(Guid postId, string userId, CancellationToken cancellationToken = default)
    {
        var existingLike = await _likeRepository.GetAsync(postId, userId, cancellationToken);
        if (existingLike is null)
            return true;

        _likeRepository.Remove(existingLike);
        await _likeRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> IsLikedAsync(Guid postId, string userId, CancellationToken cancellationToken = default)
    {
        return _likeRepository.ExistsAsync(postId, userId, cancellationToken);
    }

    public Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return _likeRepository.CountAsync(postId, cancellationToken);
    }
}
