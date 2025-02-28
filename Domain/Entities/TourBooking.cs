using Domain.Enum;
using Domain.ValueObject;

namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    public Guid UserId { get; set; }

    public Guid TourScheduleId { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
    
    public string VoucherCode { get; set; } = string.Empty;
    
    public Voucher? Voucher { get; set; }

    public decimal GrossCost { get; set; }

    public BookingStatus Status { get; set; }

    public string? Remark { get; set; }

    public virtual TourSchedule TourSchedule { get; set; } = null!;
}