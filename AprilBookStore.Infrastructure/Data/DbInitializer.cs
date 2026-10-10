using AprilBookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AprilBookStore.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedIdentityAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer");

        string[] roles = { "SuperAdmin", "Admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (roleResult.Succeeded)
                {
                    logger?.LogInformation("Role '{Role}' created successfully.", role);
                }
                else
                {
                    logger?.LogError("Failed to create role '{Role}': {Errors}", role, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

        var email = configuration["SuperAdmin:Email"];
        var password = configuration["SuperAdmin:Password"];
        var userName = configuration["SuperAdmin:UserName"] ?? email;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger?.LogWarning("SuperAdmin credentials (SuperAdmin:Email / SuperAdmin:Password) not found in configuration or user secrets. Skipping SuperAdmin user seeding.");
            return;
        }

        // 3. Ensure SuperAdmin User
        var superAdmin = await userManager.FindByEmailAsync(email);
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                BirthDate = new DateTime(1990, 1, 1)
            };

            var createResult = await userManager.CreateAsync(superAdmin, password);
            if (createResult.Succeeded)
            {
                logger?.LogInformation("SuperAdmin user '{Email}' created successfully.", email);
                var roleResult = await userManager.AddToRolesAsync(superAdmin, new[] { "SuperAdmin", "Admin" });
                if (!roleResult.Succeeded)
                {
                    logger?.LogError("Failed to assign roles to SuperAdmin user: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger?.LogError("Failed to create SuperAdmin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            foreach (var role in new[] { "SuperAdmin", "Admin" })
            {
                if (!await userManager.IsInRoleAsync(superAdmin, role))
                {
                    await userManager.AddToRoleAsync(superAdmin, role);
                    logger?.LogInformation("Assigned role '{Role}' to existing user '{Email}'.", role, email);
                }
            }
        }
    }
}
