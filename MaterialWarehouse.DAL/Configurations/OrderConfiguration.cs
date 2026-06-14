using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.State)
            .IsRequired()
            .HasConversion<string>(); // Зберігаємо статус як рядок у БД для зручності читання

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        // Зв'язок Order -> User (базовий клас)
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Налаштування доступу до інкапсульованого приватного поля _items для EF Core
        builder.Navigation(o => o.Items)
            .Metadata
            .SetField("_items");

        builder.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}