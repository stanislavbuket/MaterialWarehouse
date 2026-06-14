using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        // Налаштування TPH дискримінатора через enum властивість Role
        builder.HasDiscriminator(u => u.Role)
            .HasValue<Administrator>(UserRole.Admin)
            .HasValue<Manager>(UserRole.Manager)
            .HasValue<RegisteredUser>(UserRole.Registered);
    }
}