using InteractHub.Application.Common;
using InteractHub.Application.DTOs.Auth;

namespace InteractHub.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthUserDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}