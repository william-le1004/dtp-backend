using Domain.Enum;

namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    public Guid UserId { get; set; }

    public Guid TourScheduleId { get; set; }

    public decimal Amount { get; set; }

    public BookingStatus Status { get; set; }

    public string? Remark { get; set; }

    public virtual TourSchedule TourSchedule { get; set; } = null!;
}