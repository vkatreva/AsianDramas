using AsianDramas.Models;
using Microsoft.AspNetCore.Identity;
using static AsianDramas.Models.Drama;

namespace AsianDramas.Data
{
    public static class DataSeed
    {
        public static async Task SeedRolesAndAdmin(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager)
        {
            
            var roles = new[] { "Admin", "User", "Guest" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            string adminEmail = "admin@asiandrama.local";
            string adminPassword = "Admin@123";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                    
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

       
        public static void SeedDramas(ApplicationDbContext context)
        {
            if (!context.Dramas.Any())
            {
                context.Dramas.AddRange(
                new Drama
                {
                    Title = "Dynamite Kiss",
                    Year = 2025,
                    Region = DramaRegion.Korea,
                    Genre = "Romance",
                    Status = "Ongoing",
                    PosterUrl = "https://i0.wp.com/thefangirlverdict.com/wp-content/uploads/2025/11/Dynamite-Kiss-001.jpg?fit=595%2C831&ssl=1"
                },
                new Drama
                {
                    Title = "Moon River",
                    Year = 2025,
                    Region = DramaRegion.Korea,
                    Genre = "Fantasy",
                    Status = "Completed",
                    PosterUrl = "https://1.vikiplatform.com/c/41235c/56e95182d5.jpg?x=b"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
