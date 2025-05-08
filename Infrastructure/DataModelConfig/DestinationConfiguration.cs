using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
{
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        var currentTime = DateTime.Now;
        builder.HasData(
            new Destination
            {
                Id = Guid.NewGuid(),
                Name = "Kỳ Co",
                Latitude = "13.3456",
                Longitude = "109.1456",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Destination
            {
                Id = Guid.NewGuid(),
                Name = "Eo Gió",
                Latitude = "13.3457",
                Longitude = "109.1457",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Destination
            {
                Id = Guid.NewGuid(),
                Name = "Tháp Đôi",
                Latitude = "13.3490",
                Longitude = "109.1888",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Destination
            {
                Id = Guid.NewGuid(),
                Name = "Bãi Xép",
                Latitude = "13.3580",
                Longitude = "109.2065",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Destination
            {
                Id = Guid.NewGuid(),
                Name = "Cù Lao Xanh",
                Latitude = "13.4644",
                Longitude = "109.1999",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            }
        );
    }
}