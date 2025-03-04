using Domain.Enum;
using Domain.Extensions;
using Domain.ValueObject;

namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    public string UserId { get; private set; }
    public string Code { get; private set; }

    public Guid TourScheduleId { get; private set; }

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    public string? VoucherCode { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public Voucher? Voucher { get; private set; }

    public decimal GrossCost
    {
        get { return _tickets.Sum(x => x.GrossCost * x.Quantity); }
    }

    public decimal FinalAmount()
    {
        return GrossCost - DiscountAmount;
    }

    public BookingStatus Status { get; private set; }

    public string? Remark { get; private set; }

    public virtual TourSchedule? TourSchedule { get; private set; } = null!;

    public TourBooking(string userId, Guid tourScheduleId, TourSchedule? tourSchedule)
    {
        Code = (userId.Substring(0, 4)
                + tourScheduleId.ToString("N").Substring(0, 4)).Random();
        UserId = userId;
        TourScheduleId = tourScheduleId;
        Status = BookingStatus.Pending;
        TourSchedule = tourSchedule;
    }

    public void ApplyVoucher(Voucher? voucher)
    {
        if (voucher == null)
        {
            return;
        }

        if (!voucher.IsValid())
        {
            throw new ArgumentException("Invalid voucher");
        }

        VoucherCode = voucher.Code;
        DiscountAmount = voucher.ApplyVoucherDiscount(GrossCost);
    }

    public void AddTicket(int quantity, Guid tourScheduleTicketId)
    {
        if (!TourSchedule.HasAvailableTicket(quantity, tourScheduleTicketId))
        {
            return;
        }

        var existedTicket = _tickets.SingleOrDefault(x => x.TourBookingId == Id
                                                          && x.TourScheduleTicketId == tourScheduleTicketId);

        if (existedTicket is not null)
        {
            existedTicket.AddUnit(quantity);
        }
        else
        {
            _tickets.Add(new(tourScheduleTicketId, quantity, TourSchedule.GetGrossCost(tourScheduleTicketId)));
        }
    }

    public void CancelBooking(string remark)
    {
        if (Status == BookingStatus.Completed)
        {
            throw new AggregateException($"Can't cancel this tour booking. Status: {Status}.");
        }

        Status = BookingStatus.Cancelled;
        Remark = remark;
    }

    public void CompleteBooking(string remark)
    {
        if (Status != BookingStatus.Paid)
        {
            throw new AggregateException($"Booking wasn't paid. Status: {Status}.");
        }

        Status = BookingStatus.Completed;
        Remark = remark;
    }

    public void PurchaseBooking(string remark)
    {
        if (Status != BookingStatus.Pending)
        {
            throw new AggregateException($"Can't purchase this tour booking. Status: {Status}");
        }

        Status = BookingStatus.Completed;
        Remark = remark;
    }
}