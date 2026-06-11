using MaterialWarehouse.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaterialWarehouse.DAL.Configurations;

public class StockTransactionConfiguration
    : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.HasOne(x => x.Material)
            .WithMany()
            .HasForeignKey(x => x.MaterialId);
    }
}