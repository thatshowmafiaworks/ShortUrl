using Microsoft.AspNetCore.Identity;

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
        }


    }
}
