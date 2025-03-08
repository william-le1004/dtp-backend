using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var hasher = new PasswordHasher<User>();
        
        var adminUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9";
        var operatorUserId = "9e224968-33e4-4652-b7b7-8574d048cdb9";
        
        builder.HasData(
            new User
            {
                Id = adminUserId,
                Email = "dtpAdmin@gmail.com",
                NormalizedEmail = "DTPADMIN@GMAIL.COM",
                UserName = "dtpAdmin",
                NormalizedUserName = "DTPADMIN",
                PasswordHash = hasher.HashPassword(null, "dtpAdmin1@"),
                EmailConfirmed = true,
                Name = "Admin User",
                Address = "123 Admin St",
                PhoneNumber = "1234567890",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = operatorUserId,
                Email = "operator@gmail.com",
                NormalizedEmail = "OPERATOR@GMAIL.COM",
                UserName = "operator",
                NormalizedUserName = "OPERATOR",
                PasswordHash = hasher.HashPassword(null, "operator1@A"),
                EmailConfirmed = true,
                Name = "Operator User",
                Address = "456 Operator Rd",
                PhoneNumber = "0987654321",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}