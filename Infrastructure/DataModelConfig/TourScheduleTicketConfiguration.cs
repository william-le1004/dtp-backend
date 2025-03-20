using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class TourScheduleTicketConfiguration : IEntityTypeConfiguration<TourScheduleTicket>
{
    public void Configure(EntityTypeBuilder<TourScheduleTicket> builder)
    {
        builder.HasKey(x => new { x.TourScheduleId, x.TicketTypeId });
    }
}