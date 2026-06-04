using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasOne<RegisteredUser>()
              .WithMany(u => u.Orders)
              .HasForeignKey(o => o.UserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
