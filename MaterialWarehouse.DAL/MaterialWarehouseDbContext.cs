using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaterialWarehouse.DAL;

public class MaterialWarehouseDbContext(DbContextOptions<MaterialWarehouseDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaterialWarehouseDbContext).Assembly);
    }

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
}

public class RegisteredUserConfiguration : IEntityTypeConfiguration<RegisteredUser>
{
    public void Configure(EntityTypeBuilder<RegisteredUser> builder)
    {
        // Вказуємо EF Core, що у RegisteredUser є колекція Orders, яка зв'язана через UserId
        builder.HasMany(u => u.Orders)
               .WithOne()
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Orders)
               .Metadata
               .SetField("_orders");
    }
}