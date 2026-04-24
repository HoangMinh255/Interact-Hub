using InteractHub.Domain.Entities;
using InteractHub.Infrastructure.Options;
using InteractHub.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InteractHub.Api.Extensions;

public static class SeedingExtensions
{
    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<IdentitySeedOptions>>();

        await IdentitySeeder.SeedAsync(userManager, roleManager, seedOptions);
    }
}