using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasQueryFilter(c => !c.IsDeleted);
        var now = DateTime.Now;
        builder.HasData(new Company("Công ty Du lịch XYZ", "xyz@example.com", "0988999111", "123456789", "Hà Nội", 12)
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                CreatedBy = "System",
                LastModified = now,
                LastModifiedBy = "System",
                IsDeleted = false,
                Licensed = false
            },
            new Company("Du lịch Sài Gòn Travel", "sgtravel@example.com", "0909222333", "987654321", "TP. Hồ Chí Minh",
                15)
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                CreatedBy = "System",
                LastModified = now,
                LastModifiedBy = "System",
                IsDeleted = false,
                Licensed = false
            },
            new Company("Khám phá Việt Nam", "info@khamphavn.vn", "0912345678", "1122334455", "Đà Nẵng", 10)
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                CreatedBy = "System",
                LastModified = now,
                LastModifiedBy = "System",
                IsDeleted = false,
                Licensed = false
            });
    }
}