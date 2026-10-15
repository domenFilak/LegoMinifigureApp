using LegoMinifigureApp.Authorization;
using LegoMinifigureApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LegoMinifigureApp.Data.Seeding
{
    public class DatabaseSeeder(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<AdminAccountOptions> adminOptions,
        TimeProvider timeProvider,
        ILogger<DatabaseSeeder> logger)
    {

        private static readonly string[] StarterThemes =
        [
            "Star Wars", "Harry Potter", "Marvel Super Heroes", "DC Super Heroes",
            "Ninjago", "City", "Castle", "Pirates", "Space",
            "Collectible Minifigures", "Indiana Jones", "The Lord of the Rings",
            "Islanders", "Pirates of the Caribbean", "Minecraft", "Chima"
        ];

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminAsync();
            await SeedThemesAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                logger.LogInformation("Created role {Role}.", AppRoles.Admin);
            }
        }

        private async Task SeedAdminAsync()
        {
            var options = adminOptions.Value;

            if (string.IsNullOrWhiteSpace(options.Email) || string.IsNullOrWhiteSpace(options.Password))
            {
                logger.LogWarning("Admin account not configured – skipping admin seeding.");
                return;
            }

            var admin = await userManager.FindByEmailAsync(options.Email);

            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = options.Email,
                    Email = options.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, options.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create admin account: {errors}");
                }

                logger.LogInformation("Created admin account {Email}.", options.Email);
            }

            if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            {
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }

        private async Task SeedThemesAsync()
        {
            //IgnoreQueryFilters: soft deleted theme must not be re created on next startup
            var existingNames = await db.Themes
                .IgnoreQueryFilters([QueryFilters.SoftDelete])
                .Select(t => t.Name)
                .ToListAsync();

            var now = timeProvider.GetUtcNow().UtcDateTime;
            var added = 0;

            foreach (var name in StarterThemes.Except(existingNames, StringComparer.OrdinalIgnoreCase))
            {
                var theme = new Theme { Name = name };
                theme.MarkCreated(now);
                db.Themes.Add(theme);
                added++;
            }

            if (added > 0)
            {
                await db.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} themes.", added);
            }
        }

    }
}
