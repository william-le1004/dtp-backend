using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataModelConfig;

public class TourBookingConfiguration : IEntityTypeConfiguration<TourBooking>
{
    public void Configure(EntityTypeBuilder<TourBooking> builder)
    {
        builder.HasOne(b => b.TourSchedule) 
            .WithMany(s => s.TourBookings) 
            .HasForeignKey(b => b.TourScheduleId);
    }
}