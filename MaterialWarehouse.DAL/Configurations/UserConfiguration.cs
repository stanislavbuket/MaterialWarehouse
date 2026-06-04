using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaterialWarehouse.DAL.Entities.Users;

namespace MaterialWarehouse.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //налаштування tph наслідування для користувачів
        builder.HasDiscriminator<UserRole>("Role")
            .HasValue<Administrator>(UserRole.Admin)
            .HasValue<Manager>(UserRole.Manager)
            .HasValue<RegisteredUser>(UserRole.Registered);

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
    }
}
