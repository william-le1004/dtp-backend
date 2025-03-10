using Domain.Enum;
using Domain.Extensions;
using Domain.ValueObject;

namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    private static double Tax { get; } = 0.1;
    public string UserId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }

    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public Guid TourScheduleId { get; private set; }

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    public string? VoucherCode { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public Voucher? Voucher { get; private set; }

}