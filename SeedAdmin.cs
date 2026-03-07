using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CRMBanks.Infrastructure.Data;
using CRMBanks.Core.Entities;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("src/CRMBanks.Web/appsettings.json", optional: false)
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
var connectionString = configuration.GetConnectionString("DefaultConnection");
optionsBuilder.UseNpgsql(connectionString);

using var db = new DataContext(optionsBuilder.Options);

// Check if admin already exists
var existingAdmin = await db.Users
    .Include(u => u.Role)
    .FirstOrDefaultAsync(u => u.Role != null && u.Role.Name == "admin");

if (existingAdmin != null)
{
    Console.WriteLine("Admin account already exists!");
    Console.WriteLine($"Email: {existingAdmin.Email}");
    Console.WriteLine($"Password: admin123");
    return;
}

// Create Admin Role
var adminRole = new Role { Name = "admin" };
db.Roles.Add(adminRole);
await db.SaveChangesAsync();

// Create User Role
var userRole = new Role { Name = "user" };
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
    Email = "admin@crm.tj",
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
var defaultRegion = new Region { Name = "Душанбе" };
db.Regions.Add(defaultRegion);
await db.SaveChangesAsync();

// Add region to admin user
adminUser.Regions.Add(defaultRegion);
await db.SaveChangesAsync();

Console.WriteLine("Admin account created successfully!");
Console.WriteLine("Email: admin@crm.tj");
Console.WriteLine("Password: admin123");
Console.WriteLine("Role: admin");
