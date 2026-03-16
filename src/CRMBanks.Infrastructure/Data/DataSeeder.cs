using CRMBanks.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        // Create all regions of Tajikistan
        var regions = new List<Region>
        {
            new Region { Name = "Душанбе" },
            new Region { Name = "Хатлон" },
            new Region { Name = "Суғд" },
            new Region { Name = "Бохтар" },
            new Region { Name = "Рашт" },
            new Region { Name = "Данғара" },
            new Region { Name = "Кӯлоб" },
            new Region { Name = "Қурғонтеппа" },
            new Region { Name = "Левакант" },
            new Region { Name = "Наврӯзобод" },
            new Region { Name = "Панҷ" },
            new Region { Name = "Файзобод" },
            new Region { Name = "Турсунзода" },
            new Region { Name = "Шаҳритуз" },
            new Region { Name = "Ҳисор" },
            new Region { Name = "Искандаркуҳ" },
            new Region { Name = "Вахдат" },
            new Region { Name = "Рӯдакӣ" },
            new Region { Name = "Восеъ" },
            new Region { Name = "Бобоҷон Ғафуров" },
            new Region { Name = "Истаравшан" },
            new Region { Name = "Конибодом" },
            new Region { Name = "Исфара" },
            new Region { Name = "Канибадам" },
            new Region { Name = "Ашт" },
            new Region { Name = "Муъминобод" },
            new Region { Name = "Шамсиддин Шоҳин" },
            new Region { Name = "Балҷувон" },
            new Region { Name = "Нуробод" },
            new Region { Name = "Сангвор" },
            new Region { Name = "Тавилдара" },
            new Region { Name = "Ҷайҳун" },
            new Region { Name = "Кубодиён" },
            new Region { Name = "Абдураҳим Қосимов" },
            new Region { Name = "Чубек" },
            new Region { Name = "Хуросон" },
            new Region { Name = "Ёвон" },
            new Region { Name = "Деҳқонобод" },
            new Region { Name = "Мир Саид Алии Ҳамадонӣ" },
            new Region { Name = "Қубодиён" },
            new Region { Name = "Қумсангир" },
            new Region { Name = "Носири Хусрав" },
            new Region { Name = "Сарбанд" },
            new Region { Name = "Темурмалик" },
            new Region { Name = "Вахш" },
            new Region { Name = "Шаҳритус" }
        };
        
        foreach (var region in regions)
        {
            db.Regions.Add(region);
        }
        await db.SaveChangesAsync();

        // Add region to admin user
        var dushanbeRegion = regions.FirstOrDefault(r => r.Name == "Душанбе");
        if (dushanbeRegion != null)
        {
            adminUser.Regions.Add(dushanbeRegion);
            await db.SaveChangesAsync();
        }

        // Create Test Bank for testing
        var testBank = new Bank
        {
            Name = "Бонки Тестӣ",
            Phone = "+992 999 000 111",
            Address = "Душанбе, кӯчаи Тестӣ 123",
            Email = "test@bank.tj"
        };
        db.Banks.Add(testBank);
        await db.SaveChangesAsync();

        // Create Test User
        var testUser = new User
        {
            Name = "Корбари Тестӣ",
            Email = "kaldun597@gmail.com",
            Password = "0000",
            Phone = "+992 999 000 111",
            BankId = testBank.Id,
            RoleId = userRole.Id,
            AzSum = 0,
            ToSum = 0
        };
        db.Users.Add(testUser);
        await db.SaveChangesAsync();

        // Add region to test user
        if (dushanbeRegion != null)
        {
            testUser.Regions.Add(dushanbeRegion);
            await db.SaveChangesAsync();
        }

        // Create pre-configured 2FA code for test user (111111)
        var testAuth2F = new Auth2F
        {
            UserId = testUser.Id,
            Code = 111111,
            DateTimeSendCode = DateTimeOffset.UtcNow,
            IsEnabled = true
        };
        db.Auth2Fs.Add(testAuth2F);
        await db.SaveChangesAsync();
    }
}
