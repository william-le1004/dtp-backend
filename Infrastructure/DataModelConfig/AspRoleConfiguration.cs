using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class AspRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Tourist", NormalizedName = "TOURIST" },
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Manager", NormalizedName = "MANAGER" },
            new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Operator", NormalizedName = "OPERATOR" }
        );
    }
}