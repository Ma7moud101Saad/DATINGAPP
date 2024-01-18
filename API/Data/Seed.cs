using API.DTOs;
using API.Entites;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser>userManager,RoleManager<AppRole> roleManager) {
            if (await userManager.Users.AnyAsync()) return;

           var userData=await File.ReadAllTextAsync("Data/UserSeedData.json");

           var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData,options);

            List<AppRole> appRoles = new List<AppRole>() {
                new AppRole{Name="Member" },
                new AppRole{Name="Admin" },
                new AppRole{Name="Moderator" }
            };
            foreach (var appRole in appRoles)
            {
                await roleManager.CreateAsync(appRole);
            }

            foreach (var user in users) {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user,"Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

          
            var admin = new AppUser {UserName="Admin" };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

        }
    }
}
