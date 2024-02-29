using System.Text.Json;
using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Seeds
{
    public class Seed
    {
            public static async Task SeedUsers(UserManager<User> userManager)
    {

        if (await userManager.Users.AnyAsync()) return;

        var usersData = await System.IO.File.ReadAllTextAsync("Seeds/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<User>>(usersData);

        foreach (var user in users)
        {


            user.UserName = user.UserName.ToLower();


           await userManager.CreateAsync(user,"Pa$$w0rd");
        }
    }
    }
}