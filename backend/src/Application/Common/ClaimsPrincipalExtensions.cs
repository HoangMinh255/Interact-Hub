using System.Security.Claims;

namespace InteractHub.Application.Common;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        return principal.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? principal.FindFirstValue(AppConstants.Claims.UserId);
    }

    public static string GetRequiredUserId(this ClaimsPrincipal? principal)
    {
        var userId = principal.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User id claim is missing.");
        }

        return userId;
    }

    public static string? GetEmail(this ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        return principal.FindFirstValue(ClaimTypes.Email)
               ?? principal.FindFirstValue(AppConstants.Claims.Email);
    }

    public static string? GetFullName(this ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        return principal.FindFirstValue(AppConstants.Claims.FullName);
    }

    public static bool IsAdminUser(this ClaimsPrincipal? principal)
        => principal?.IsInRole(AppConstants.Roles.Admin) == true;
}