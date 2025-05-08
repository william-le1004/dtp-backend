using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.Ignore(e => e.AvailableVoucher).HasKey(x => x.Id);

        builder.HasData(new Voucher
            {
                Id = Guid.NewGuid(),
                Code = "8OMPKJGX2P",
                MaxDiscountAmount = 100000,
                Percent = 0.1,
                ExpiryDate = new DateTime(2025, 12, 31, 23, 59, 59),
                Quantity = 100,
                Description = "Giảm 10% tối đa 100K",
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            },
            new Voucher
            {
                Id = Guid.NewGuid(),
                Code = "8OMPKJGX2F",
                MaxDiscountAmount = 150000,
                Percent = 0.2,
                ExpiryDate = new DateTime(2025, 12, 31, 23, 59, 59),
                Quantity = 50,
                Description = "Giảm 20% tối đa 150K",
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            },
            new Voucher
            {
                Id = Guid.NewGuid(),
                Code = "8OMPKJGXE3",
                MaxDiscountAmount = 200000,
                Percent = 0.5,
                ExpiryDate = new DateTime(2025, 6, 30, 23, 59, 59),
                Quantity = 20,
                Description = "Giảm 50% tối đa 200K",
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            });
    }
}