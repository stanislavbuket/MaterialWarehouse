using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities;

namespace MaterialWarehouse.DAL.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.HasOne(oi => oi.Material)
              .WithMany()
              .HasForeignKey(oi => oi.MaterialId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
