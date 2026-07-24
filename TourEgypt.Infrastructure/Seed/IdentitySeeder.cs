using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TourEgypt.Core.Entities;

namespace TourEgypt.Infrastructure.Seed
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            // Create Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = "Admin"
                });
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = "User"
                });
            }

            // Create Default Admin
            var admin = await userManager.FindByEmailAsync("admin@touregypt.com");

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "Menna",
                    Email = "admin@touregypt.com",
                    FirstName = "Menna",
                    LastName = "Reda",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
