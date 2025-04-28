using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        var adminUserId = "8e445865-a24d-4543-a6c6-9443d048cdb9";
        var operatorUserId = "9e224968-33e4-4652-b7b7-8574d048cdb9";

        builder.HasData(
            new Wallet(adminUserId, 1000) { Id = Guid.NewGuid() },
            new Wallet(operatorUserId, 500) { Id = Guid.NewGuid() }
        );
    }
}