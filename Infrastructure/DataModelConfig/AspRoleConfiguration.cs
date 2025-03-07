using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class AspRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole { Id = "000b8fc6-be6c-44ad-9d48-9e7dad7c1221", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "096c07cd-26b0-4ee2-8083-efb870889599", Name = "Tourist", NormalizedName = "TOURIST" },
            new IdentityRole { Id = "d856e718-a5a2-4dea-80e5-10e4dfc995e2", Name = "Manager", NormalizedName = "MANAGER" },
            new IdentityRole { Id = "f8efc705-4d2f-40b8-9959-013505b24f86", Name = "Operator", NormalizedName = "OPERATOR" }
        );
    }
}