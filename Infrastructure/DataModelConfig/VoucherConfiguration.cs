using Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.Ignore(e => e.AvailableVoucher).HasKey(x => x.Id);
    }
}