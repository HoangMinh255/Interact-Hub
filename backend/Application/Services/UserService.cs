using InteractHub.Application.Common;
using InteractHub.Application.DTOs.User;
using InteractHub.Application.Interfaces.Infrastructure;
using InteractHub.Application.Interfaces.Repositories;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using InteractHub.Application.Common.Exceptions;

namespace InteractHub.Application.Services;

public sealed class UserService : IUserService
{
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;

    public UserService(
        IUserRepository userRepository,
        IFileStorageService fileStorageService)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
    }

    public Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByIdAsync(userId, cancellationToken);
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByEmailAsync(email, cancellationToken);
    }

    public Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByUserNameAsync(userName, cancellationToken);
    }

    public Task<PagedResult<ApplicationUser>> SearchAsync(UserSearchQueryDto query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var keyword = string.IsNullOrWhiteSpace(query.Keyword)
            ? null
            : query.Keyword.Trim();

        return _userRepository.SearchAsync(
            keyword,
            query.Page,
            query.PageSize,
            includeInactive: false,
            cancellationToken);
    }

    public async Task<ApplicationUser> UpdateProfileAsync(
        string userId,
        UpdateUserProfileDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var fullName = dto.FullName.Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BadRequestException("Full name is required.");
        }

        if (dto.DateOfBirth.HasValue && dto.DateOfBirth.Value.Date > DateTime.UtcNow.Date)
        {
            throw new BadRequestException("Date of birth cannot be in the future.");
        }

        user.FullName = fullName;
        user.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();
        user.DateOfBirth = dto.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<ApplicationUser> UploadAvatarAsync(
        string userId,
        Stream avatarStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (avatarStream is null)
        {
            throw new BadRequestException("Avatar file stream is required.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BadRequestException("Avatar file name is required.");
        }

        if (string.IsNullOrWhiteSpace(contentType) || !AllowedImageContentTypes.Contains(contentType))
        {
            throw new BadRequestException("Unsupported avatar image type.");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var (_, url) = await _fileStorageService.UploadAsync(
            avatarStream,
            fileName,
            contentType,
            "avatars",
            cancellationToken);

        user.AvatarUrl = url;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> DeactivateAsync(string targetUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!user.IsActive)
        {
            return false;
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ReactivateAsync(string targetUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (user.IsActive)
        {
            return false;
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
