using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class TourDestinationConfiguration : IEntityTypeConfiguration<TourDestination>
{
    public void Configure(EntityTypeBuilder<TourDestination> builder)
    {
        builder.HasKey(x => x.Id);
    }
}