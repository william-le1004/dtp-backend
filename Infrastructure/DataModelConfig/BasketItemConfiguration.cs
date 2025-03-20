using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class BasketItemConfiguration : IEntityTypeConfiguration<TourBasketItem>
{
    public void Configure(EntityTypeBuilder<TourBasketItem> builder)
    {
        builder.HasKey(x => new { x.TourScheduleId, x.BasketId, TourScheduleTicketId = x.TicketTypeId });
    }
}