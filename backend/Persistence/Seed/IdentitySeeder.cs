using InteractHub.Application.Common;
using InteractHub.Domain.Entities;
using InteractHub.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace InteractHub.Persistence.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentitySeedOptions> seedOptions,
        CancellationToken cancellationToken = default)
    {
        var roles = new[] { AppConstants.Roles.User, AppConstants.Roles.Admin };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = seedOptions.Value;

        var existingAdmin = await userManager.FindByEmailAsync(admin.Email);
        if (existingAdmin is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = admin.Email,
                Email = admin.Email,
                FullName = admin.FullName,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, admin.Password);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, AppConstants.Roles.Admin);
            }
        }
    }
}