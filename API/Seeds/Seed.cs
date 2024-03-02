using System.Text.Json;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Seeds
{
    public class Seed
    {
            public static async Task SeedUsers(UserManager<User> userManager,RoleManager<AppRole> roleManager)
    {

        if (await userManager.Users.AnyAsync()) return;

        var usersData = await System.IO.File.ReadAllTextAsync("Seeds/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<User>>(usersData);

        var roles= new List<AppRole>
        {
            new AppRole{Name="Member"},
            new AppRole{Name="Admin"},
            new AppRole{Name="Moderator"},
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {


            user.UserName = user.UserName.ToLower();


           await userManager.CreateAsync(user,"Pa$$w0rd");
           await userManager.AddToRoleAsync(user,"Member");
        }

        var admin= new User{
            UserName="admin"
        };
        await userManager.CreateAsync(admin,"Pa$$w0rd");
        await userManager.AddToRoleAsync(admin,"Admin");
        await userManager.AddToRoleAsync(admin,"Moderator");
    }
    }
}