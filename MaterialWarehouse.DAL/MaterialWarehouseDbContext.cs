using Microsoft.EntityFrameworkCore;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;

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
}
