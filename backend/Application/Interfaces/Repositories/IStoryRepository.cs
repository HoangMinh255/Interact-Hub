using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Repositories;

public interface IStoryRepository
{
    Task<Story?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Story>> GetActiveByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Story>> GetFeedAsync(IEnumerable<string> userIds, int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(Story story, CancellationToken cancellationToken = default);

    void Update(Story story);

    void Remove(Story story);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
