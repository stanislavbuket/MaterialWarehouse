using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities;

namespace MaterialWarehouse.DAL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        //зв'язок категорія-матеріали
        builder.HasMany(c => c.Materials)
              .WithOne(m => m.Category)
              .HasForeignKey(m => m.CategoryId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
