using InteractHub.Application.DTOs.Story;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Services;

public sealed class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;
    private readonly IFileStorageService _fileStorageService;

    public StoryService(
        IStoryRepository storyRepository,
        IFileStorageService fileStorageService)
    {
        _storyRepository = storyRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Story> CreateAsync(CreateStoryDto dto, string userId, CancellationToken cancellationToken = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));

        var hasContent = !string.IsNullOrWhiteSpace(dto.Content);
        var hasMedia = !string.IsNullOrWhiteSpace(dto.MediaUrl);

        if (!hasContent && !hasMedia)
            throw new ArgumentException("Story must have content or media.", nameof(dto));

        var expireAt = dto.ExpireAt ?? DateTime.UtcNow.AddHours(24);
        if (expireAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpireAt must be in the future.", nameof(dto));

        var story = new Story
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = dto.Content?.Trim(),
            MediaUrl = dto.MediaUrl,
            MediaType = (byte) dto.MediaType,
            ExpireAt = expireAt,
            IsActive = true
        };

        await _storyRepository.AddAsync(story, cancellationToken);
        await _storyRepository.SaveChangesAsync(cancellationToken);

        return story;
    }

    public async Task<Story?> UpdateAsync(Guid storyId, string userId, UpdateStoryDto dto, CancellationToken cancellationToken = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));

        var story = await _storyRepository.GetByIdAsync(storyId, cancellationToken);
        if (story is null) return null;

        if (!string.Equals(story.UserId, userId, StringComparison.Ordinal))
            return null;

        if (dto.Content is not null)
            story.Content = dto.Content.Trim();

        if (dto.MediaUrl is not null)
            story.MediaUrl = dto.MediaUrl;

            story.MediaType = (byte) dto.MediaType;

        if (dto.ExpireAt.HasValue)
        {
            if (dto.ExpireAt.Value <= DateTime.UtcNow)
                throw new ArgumentException("ExpireAt must be in the future.", nameof(dto));

            story.ExpireAt = dto.ExpireAt.Value;
        }

        if (dto.IsActive.HasValue)
            story.IsActive = dto.IsActive.Value;

        _storyRepository.Update(story);
        await _storyRepository.SaveChangesAsync(cancellationToken);

        return story;
    }

    public async Task<bool> DeleteAsync(Guid storyId, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));

        var story = await _storyRepository.GetByIdAsync(storyId, cancellationToken);
        if (story is null) return false;

        if (!string.Equals(story.UserId, userId, StringComparison.Ordinal))
            return false;

        _storyRepository.Remove(story);
        await _storyRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> HideAsync(Guid storyId, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));

        var story = await _storyRepository.GetByIdAsync(storyId, cancellationToken);
        if (story is null) return false;

        if (!string.Equals(story.UserId, userId, StringComparison.Ordinal))
            return false;

        story.IsActive = false;
        _storyRepository.Update(story);
        await _storyRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Story>> GetActiveStoriesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Array.Empty<Story>();

        var stories = await _storyRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        return stories
            .Where(x => x.IsActive && x.ExpireAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<Story>> GetFeedAsync(string userId, int page, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Array.Empty<Story>();

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        // Phase-3 first batch: feed is scoped to active stories of the current user.
        // Later we can expand to friend network-based feed once friendship rules are wired in.
        var storyIds = new[] { userId };
        var stories = await _storyRepository.GetFeedAsync(storyIds, page, pageSize, cancellationToken);

        return stories
            .Where(x => x.IsActive && x.ExpireAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
}
