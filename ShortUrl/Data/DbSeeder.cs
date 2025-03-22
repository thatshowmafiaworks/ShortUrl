using Microsoft.AspNetCore.Identity;
using ShortUrl.Models;

namespace ShortUrl.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            var admin = new IdentityUser
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                EmailConfirmed = true
            };

            var userInDb = await userManager.FindByEmailAsync(admin.Email);

            if (userInDb is null)
            {
                await userManager.CreateAsync(admin, "Admin_123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var context = services.GetRequiredService<ApplicationDbContext>();
            var about = """
                This website generates a fixed-length hash (8 characters) from an original URL using SHA-256.
                The goal is to create a unique and URL-friendly identifier for a given input URL, which can be used as a shortened link.
                """;
            if (context.About.Count() == 0)
            {
                AboutText text = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = about,
                    CreatedBy = "",
                    Created = DateTime.UtcNow
                };
                context.About.Add(text);
                await context.SaveChangesAsync();
            }
        }


    }
}
