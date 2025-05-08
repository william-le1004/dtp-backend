using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        var currentTime = DateTime.Now;
        builder.HasData(
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tour 1 ngày",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tour 3 ngày",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tour 7 ngày",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tour trong ngày",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "Tour nửa ngày",
                IsDeleted = false,
                CreatedAt = currentTime,
                CreatedBy = "admin",
                LastModified = currentTime,
                LastModifiedBy = "admin"
            }
        );
    }
}