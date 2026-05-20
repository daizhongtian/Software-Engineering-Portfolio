using CinemaMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CinemaMvc.Data
{
    public static class SeedData
    {
        private sealed record AdminSeed(
            string Email,
            string Password,
            string FirstName,
            string LastName,
            string? PhoneNumber);

        private static readonly AdminSeed[] AdminAccounts =
        {
            new("daiadmin@admin.com", "20031018;", "Dai", "Admin", null),
            new("daiadmin2@admin.com", "daizhongtian1.", "Dai", "Admin2", null)
        };

        public static async Task InitializeAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();

            await context.Database.MigrateAsync();
            await EnsureRolesAsync(roleManager);
            await EnsureAdminAccountsAsync(userManager, passwordHasher);

            if (!context.Cinemas.Any())
            {
                context.Cinemas.AddRange(
                    new Cinema { Name = "Cinema A", Rows = 5, SeatsPerRow = 8 },
                    new Cinema { Name = "Cinema B", Rows = 8, SeatsPerRow = 10 },
                    new Cinema { Name = "Cinema C", Rows = 6, SeatsPerRow = 12 }
                );
            }

            await context.SaveChangesAsync();
        }

        public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        public static async Task EnsureAdminAccountsAsync(
            UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            foreach (var adminSeed in AdminAccounts)
            {
                var user = await userManager.FindByEmailAsync(adminSeed.Email)
                    ?? await userManager.FindByNameAsync(adminSeed.Email);

                if (user == null)
                {
                    user = new ApplicationUser();
                    ApplyAdminValues(user, adminSeed, passwordHasher);

                    var createResult = await userManager.CreateAsync(user);
                    EnsureSuccess(createResult, $"create admin account '{adminSeed.Email}'");
                }
                else
                {
                    ApplyAdminValues(user, adminSeed, passwordHasher);

                    var updateResult = await userManager.UpdateAsync(user);
                    EnsureSuccess(updateResult, $"update admin account '{adminSeed.Email}'");
                }

                if (!await userManager.IsInRoleAsync(user, "Admin"))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                    EnsureSuccess(addToRoleResult, $"assign Admin role to '{adminSeed.Email}'");
                }
            }

        }

        private static void ApplyAdminValues(
            ApplicationUser user,
            AdminSeed adminSeed,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            user.UserName = adminSeed.Email;
            user.Email = adminSeed.Email;
            user.NormalizedUserName = adminSeed.Email.ToUpperInvariant();
            user.NormalizedEmail = adminSeed.Email.ToUpperInvariant();
            user.EmailConfirmed = true;
            user.FirstName = adminSeed.FirstName;
            user.LastName = adminSeed.LastName;
            user.PhoneNumber = adminSeed.PhoneNumber;
            user.PasswordHash = passwordHasher.HashPassword(user, adminSeed.Password);
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        private static void EnsureSuccess(IdentityResult result, string operation)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to {operation}. {errors}");
        }
    }
}
