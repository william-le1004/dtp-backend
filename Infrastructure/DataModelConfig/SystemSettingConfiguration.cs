using Domain.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class SystemSettingConfiguration :IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.Property(o => o.SettingCode)
            .HasConversion<string>();
        builder.Property(o => o.SettingKey)
            .HasConversion<string>();
    }
}