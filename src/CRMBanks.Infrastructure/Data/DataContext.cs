using CRMBanks.Core.Entities;
using CRMBanks.SharedKernel.Common.AbstractClasses;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestAction> RequestActions { get; set; }
    public DbSet<Credit> Credits { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<TypeCredit> TypeCredits { get; set; }
    public DbSet<TypeDeposit> TypeDeposits { get; set; }
    public DbSet<TypeSum> TypeSums { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<TypeProduct> TypeProducts { get; set; }
    public DbSet<Auth2F> Auth2Fs { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntityProduction).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = System.Linq.Expressions.Expression.Property(parameter, nameof(EntityProduction.IsDeleted));
                var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                var body = System.Linq.Expressions.Expression.Equal(isDeletedProperty, falseConstant);
                var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        modelBuilder.Entity<User>()
            .HasOne(u => u.Bank)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BankId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Requests)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.Region)
            .WithMany(r => r.Requests)
            .HasForeignKey(r => r.RegionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Request>()
            .HasOne(r => r.TypeProductinfo)
            .WithMany(t => t.Requests)
            .HasForeignKey(r => r.TypeProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TypeProduct>()
            .HasOne(t => t.TypeCredit)
            .WithMany(tc => tc.TypeProducts)
            .HasForeignKey(t => t.TypeCreditId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TypeProduct>()
            .HasOne(t => t.TypeDeposit)
            .WithMany(td => td.TypeProducts)
            .HasForeignKey(t => t.TypeDepositId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RequestAction>()
            .HasOne(ra => ra.Request)
            .WithMany(r => r.RequestActions)
            .HasForeignKey(ra => ra.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RequestAction>()
            .HasOne(ra => ra.Users)
            .WithMany(u => u.RequestActions)
            .HasForeignKey(ra => ra.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Auth2F>()
            .HasOne(a => a.Users)
            .WithMany(u => u.Auth2Fs)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Request)
            .WithMany()
            .HasForeignKey(n => n.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "admin" },
            new Role { Id = 2, Name = "user" }
        );
    }
}
