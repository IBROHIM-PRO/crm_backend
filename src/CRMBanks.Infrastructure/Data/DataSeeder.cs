using CRMBanks.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(DataContext db)
    {
        // Check if database has any data
        if (await db.Roles.AnyAsync())
        {
            return; // Database already seeded
        }

        // Create Admin Role
        var adminRole = new Role
        {
            Name = "admin"
        };
        db.Roles.Add(adminRole);
        await db.SaveChangesAsync();

        // Create User Role
        var userRole = new Role
        {
            Name = "user"
        };
        db.Roles.Add(userRole);
        await db.SaveChangesAsync();

        // Create a default bank for admin
        var adminBank = new Bank
        {
            Name = "Бонки Марказӣ",
            Phone = "+992 000 000 001",
            Address = "Душанбе, хиёбони Рудакӣ",
            Email = "admin@crm.tj"
        };
        db.Banks.Add(adminBank);
        await db.SaveChangesAsync();

        // Create Admin User
        var adminUser = new User
        {
            Name = "Администратор",
            Email = "kaldun5",
            Password = "admin123",
            Phone = "+992 000 000 001",
            BankId = adminBank.Id,
            RoleId = adminRole.Id,
            AzSum = 0,
            ToSum = 0
        };
        db.Users.Add(adminUser);
        await db.SaveChangesAsync();

        // Create a default region
        var defaultRegion = new Region
        {
            Name = "Душанбе"
        };
        db.Regions.Add(defaultRegion);
        await db.SaveChangesAsync();

        // Add region to admin user
        adminUser.Regions.Add(defaultRegion);
        await db.SaveChangesAsync();
    }
}
