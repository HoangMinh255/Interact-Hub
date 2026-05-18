using InteractHub.Application.Common;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(
        string? keyword,
        int page,
        int pageSize,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);
    void Update(ApplicationUser user);
    void Remove(ApplicationUser user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
