using InteractHub.Application.Common;
using InteractHub.Application.DTOs.User;
using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> SearchAsync(UserSearchQueryDto query, CancellationToken cancellationToken = default);
    Task<ApplicationUser> UpdateProfileAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default);
    Task<ApplicationUser> UploadAvatarAsync(
        string userId,
        Stream avatarStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(string targetUserId, CancellationToken cancellationToken = default);
    Task<bool> ReactivateAsync(string targetUserId, CancellationToken cancellationToken = default);
}
