using InteractHub.Application.DTOs.Story;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;

public interface IStoryService
{
    Task<Story> CreateAsync(CreateStoryDto dto, string userId, CancellationToken cancellationToken = default);

    Task<Story?> UpdateAsync(Guid storyId, string userId, UpdateStoryDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid storyId, string userId, CancellationToken cancellationToken = default);

    Task<bool> HideAsync(Guid storyId, string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Story>> GetActiveStoriesByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Story>> GetFeedAsync(string userId, int page, int pageSize = 10, CancellationToken cancellationToken = default);
}
