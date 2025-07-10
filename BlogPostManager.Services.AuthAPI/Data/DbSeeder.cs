using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogPostManager.Services.AuthAPI.Models;

namespace BlogPostManager.Services.AuthAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Ensure DB is created and migrated
            await context.Database.MigrateAsync();

            // 2. Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 3. Create default admin user
            var adminEmail = "admin@blog.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    Id = "admin-fixed-guid", // use a fixed ID so BlogPostService can reference it
                    UserName = adminEmail,
                    Name = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123*"); // choose a secure password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
