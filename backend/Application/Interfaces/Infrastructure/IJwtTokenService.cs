using InteractHub.Domain.Entities;

namespace InteractHub.Application.Interfaces.Infrastructure;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
}