using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MyRazorApp.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

          
            var adminEmail = "admin@example.com";
            var adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = adminEmail,
                NormalizedUserName = "ADMIN",
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "admin123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

         
            var userEmail = "user@example.com";
            var normalUser = new IdentityUser
            {
                UserName = "user",
                Email = userEmail,
                NormalizedUserName = "USER",
                NormalizedEmail = userEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var result = await userManager.CreateAsync(normalUser, "user123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }
}
