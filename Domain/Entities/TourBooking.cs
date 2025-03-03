using Domain.Enum;
using Domain.ValueObject;

namespace Domain.Entities;

public partial class TourBooking : AuditEntity
{
    public string UserId { get; private set; }

    public Guid TourScheduleId { get; private set; }

    private readonly List<Ticket> tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => tickets.AsReadOnly();

    public string VoucherCode { get; private set; } = string.Empty;

    public Voucher? Voucher { get; private set; }

    public decimal GrossCost
    {
        get { return tickets.Sum(x => x.GrossCost * x.Quantity); }
    }

    public BookingStatus Status { get; private set; }

    public string? Remark { get; private set; }

    public virtual TourSchedule TourSchedule { get; private set; } = null!;

    public TourBooking(string userId, Guid tourScheduleId, Voucher? voucher, BookingStatus status,
        string? remark, TourSchedule tourSchedule)
    {
        UserId = userId;
        TourScheduleId = tourScheduleId;
        Voucher = voucher;
        Status = BookingStatus.Pending;
        TourSchedule = tourSchedule;
    }

    public void AddTicket(Ticket ticket, int quantity, Guid tourScheduleTicketId, decimal grossCost)
    {
        if (!TourSchedule.HasAvailableTicket(quantity, tourScheduleTicketId))
        {
            return;
        }

        var existedTicket = tickets.SingleOrDefault(x => x.TourBookingId == Id
                                                         && x.TourScheduleTicketId == tourScheduleTicketId);

        if (existedTicket is not null)
        {
            existedTicket.AddUnit(quantity);
        }
        else
        {
            tickets.Add(new(tourScheduleTicketId, quantity, grossCost));
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