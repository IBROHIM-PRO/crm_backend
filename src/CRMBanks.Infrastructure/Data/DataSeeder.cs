using CRMBanks.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CRMBanks.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(DataContext db)
    {
        // 1. Тафтиш: Агар корбарон аллакай бошанд, сидингро иҷро намекунем
        // (ролҳо тавассути HasData эҷод мешаванд, бинобар ин танҳо корбаронро санҷем)
        if (await db.Users.AnyAsync())
        {
            return;
        }

        using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            // --- 2. Гирифтани Ролҳо ---
            var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
            var userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
            var bossRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "boss");
            var workerRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "worker");
            var clientRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "client");
            
            if (adminRole == null || userRole == null || bossRole == null || workerRole == null || clientRole == null)
            {
                throw new InvalidOperationException("Required roles not found in database. Check HasData configuration.");
            }

            // --- 3. Эҷоди Бонкҳо ---
            var adminBank = new Bank
            {
                Name = "Бонки Марказӣ",
                Phone = "+992 000 000 001",
                Address = "Душанбе, хиёбони Рудакӣ",
                Email = "admin@crm.tj"
            };

            var testBank = new Bank
            {
                Name = "Бонки Тестӣ",
                Phone = "+992 999 000 111",
                Address = "Душанбе, кӯчаи Тестӣ 123",
                Email = "test@bank.tj"
            };

            await db.Banks.AddRangeAsync(adminBank, testBank);
            await db.SaveChangesAsync();

            // --- 4. Эҷоди Регионҳо ---
            var regions = new List<Region>
            {
                new Region { Name = "Душанбе" }, new Region { Name = "Хатлон" },
                new Region { Name = "Суғд" }, new Region { Name = "Бохтар" },
                new Region { Name = "Рашт" }, new Region { Name = "Данғара" },
                new Region { Name = "Кӯлоб" }, new Region { Name = "Қурғонтеппа" },
                new Region { Name = "Левакант" }, new Region { Name = "Наврӯзобод" },
                new Region { Name = "Панҷ" }, new Region { Name = "Файзобод" },
                new Region { Name = "Турсунзода" }, new Region { Name = "Шаҳритуз" },
                new Region { Name = "Ҳисор" }, new Region { Name = "Искандаркуҳ" },
                new Region { Name = "Вахдат" }, new Region { Name = "Рӯдакӣ" },
                new Region { Name = "Восеъ" }, new Region { Name = "Бобоҷон Ғафуров" },
                new Region { Name = "Истаравшан" }, new Region { Name = "Конибодом" },
                new Region { Name = "Исфара" }, new Region { Name = "Ашт" },
                new Region { Name = "Муъминобод" }, new Region { Name = "Шамсиддин Шоҳин" },
                new Region { Name = "Балҷувон" }, new Region { Name = "Нуробод" },
                new Region { Name = "Сангвор" }, new Region { Name = "Тавилдара" },
                new Region { Name = "Ҷайҳун" }, new Region { Name = "Кубодиён" },
                new Region { Name = "Хуросон" }, new Region { Name = "Ёвон" }
            };

            await db.Regions.AddRangeAsync(regions);
            await db.SaveChangesAsync();

            var dushanbeRegion = regions.First(r => r.Name == "Душанбе");

            // --- 5. Эҷоди Корбарон ---
            // Админ
            var adminUser = new User
            {
                Name = "Администратор",
                Email = "kaldun597@gmail.com",
                Password = "0000",
                Phone = "+992 000 000 001",
                BankId = adminBank.Id,
                RoleId = adminRole.Id,
                AzSum = 0,
                ToSum = 0,
                MaxLoanAmount = 1000000, // 1 million for admin
                Token = Guid.NewGuid().ToString(),
                Regions = new List<Region> { dushanbeRegion }
            };

            // Boss
            var bossUser = new User
            {
                Name = "Раҳбар",
                Email = "boss@crm.tj",
                Password = "123456",
                Phone = "+992 000 000 002",
                BankId = adminBank.Id,
                RoleId = bossRole.Id,
                AzSum = 0,
                ToSum = 0,
                MaxLoanAmount = 2000000, // 2 million for boss
                Token = Guid.NewGuid().ToString(),
                Regions = regions.Take(5).ToList() // Boss can see first 5 regions
            };

            // Worker
            var workerUser = new User
            {
                Name = "Коргар",
                Email = "worker@crm.tj",
                Password = "123456",
                Phone = "+992 000 000 003",
                BankId = testBank.Id,
                RoleId = workerRole.Id,
                AzSum = 0,
                ToSum = 0,
                MaxLoanAmount = 500000, // 500k for worker
                Token = Guid.NewGuid().ToString(),
                Regions = new List<Region> { dushanbeRegion } // Worker only sees Dushanbe
            };

            // Client
            var clientUser = new User
            {
                Name = "Мизоҷ",
                Email = "client@crm.tj",
                Password = "123456",
                Phone = "+992 000 000 004",
                BankId = testBank.Id,
                RoleId = clientRole.Id,
                AzSum = 0,
                ToSum = 0,
                MaxLoanAmount = 0, // Clients don't have approval limits
                Token = Guid.NewGuid().ToString(),
                Regions = new List<Region> { dushanbeRegion }
            };
            
            await db.Users.AddRangeAsync(adminUser, bossUser, workerUser, clientUser);
            await db.SaveChangesAsync();

            // --- 6. Эҷоди 2FA барои ҳамаи корбарон ---
            var auth2Fs = new List<Auth2F>
            {
                new() { UserId = adminUser.Id, Code = 111111, DateTimeSendCode = DateTimeOffset.UtcNow, IsEnabled = true },
                new() { UserId = bossUser.Id, Code = 111111, DateTimeSendCode = DateTimeOffset.UtcNow, IsEnabled = true },
                new() { UserId = workerUser.Id, Code = 111111, DateTimeSendCode = DateTimeOffset.UtcNow, IsEnabled = true },
                new() { UserId = clientUser.Id, Code = 111111, DateTimeSendCode = DateTimeOffset.UtcNow, IsEnabled = true }
            };
            
            await db.Auth2Fs.AddRangeAsync(auth2Fs);
            await db.SaveChangesAsync();

            // Тасдиқи ҳамаи амалиётҳо
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Дар ин ҷо метавонед хатогиро лог кунед
            throw; 
        }
    }
}