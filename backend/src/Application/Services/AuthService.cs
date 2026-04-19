using InteractHub.Application.Common;
using InteractHub.Application.DTOs.Auth;
using InteractHub.Application.Interfaces.Infrastructure;
using InteractHub.Application.Interfaces.Services;
using InteractHub.Domain.Entities;
using InteractHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InteractHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IdentitySeedOptions _seedOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IOptions<IdentitySeedOptions> seedOptions)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _seedOptions = seedOptions.Value;
    }

    public async Task<ApiResponse<AuthUserDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail is not null)
        {
            return ApiResponse<AuthUserDto>.Fail("Registration failed", ["Email already exists."]);
        }

        var existingByUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingByUserName is not null)
        {
            return ApiResponse<AuthUserDto>.Fail("Registration failed", ["Username already exists."]);
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return ApiResponse<AuthUserDto>.Fail(
                "Registration failed",
                result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, AppConstants.Roles.User);

        var response = new AuthUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = [AppConstants.Roles.User]
        };

        return ApiResponse<AuthUserDto>.Ok(response, "User registered successfully");
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return ApiResponse<LoginResponseDto>.Fail("Login failed", ["Invalid email or password."]);
        }

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponseDto>.Fail("Login failed", ["User account is inactive."]);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return ApiResponse<LoginResponseDto>.Fail("Login failed", ["Invalid email or password."]);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60),
            User = new AuthUserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToArray()
            }
        };

        return ApiResponse<LoginResponseDto>.Ok(response, "Login successful");
    }
}