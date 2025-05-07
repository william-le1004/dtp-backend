using Domain.Enum;
using Domain.Events;
using Domain.Extensions;
using Domain.ValueObject;

namespace Domain.Entities;

public partial class TourBooking : Entity
{
    public string UserId { get; private set; }
    public string Code { get; init; }
    
    public long RefCode { get; private set; }
    public string Name { get; private set; }

    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public Guid TourScheduleId { get; private set; }

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    public string? VoucherCode { get; private set; }

    public decimal DiscountAmount { get; private set; }
    
    public virtual User User { get; set; } = null!;

    public decimal GrossCost
    {
        get { return _tickets.Sum(x => x.GrossCost * x.Quantity); }
    }

    public decimal NetCost() => GrossCost - DiscountAmount;


    public BookingStatus Status { get; private set; }

    public string? Remark { get; private set; }

    public virtual TourSchedule TourSchedule { get; set; } = null!;
    public virtual Rating? Rating { get; set; } = null!;
    

    public TourBooking()
    {
    }

    public TourBooking(string userId, Guid tourScheduleId, TourSchedule tourSchedule, string name, string phoneNumber,
        string email)
    {
        Code = (userId.Substring(0, 6)
                + tourScheduleId.ToString("N").Substring(0, 6)).Random();
        RefCode = Code.ToLong();
        UserId = userId;
        TourScheduleId = tourScheduleId;
        Status = BookingStatus.Submitted;
        TourSchedule = tourSchedule;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public void ApplyVoucher(Voucher voucher)
    {
        if (!voucher.IsValid())
        {
            throw new ArgumentException("Invalid voucher");
        }

        VoucherCode = voucher.Code;
        DiscountAmount = voucher.ApplyVoucherDiscount(GrossCost);
    }

    public void AddTicket(int quantity, Guid ticketTypeId)
    {
        if (TourSchedule.IsStarted())
        {
            throw new AggregateException("Tour schedule is already started");
        }

        if (!TourSchedule.HasAvailableTicket(quantity, ticketTypeId))
        {
            throw new AggregateException("Ticket quantity is out of range");
        }

        var existedTicket = _tickets.SingleOrDefault(x => x.TourBookingId == Id
                                                          && x.TicketTypeId == ticketTypeId);

        if (existedTicket is not null)
        {
            existedTicket.AddUnit(quantity);
        }
        else
        {
            _tickets.Add(new(ticketTypeId, quantity, TourSchedule.GetGrossCost(ticketTypeId)));
        }
    }

    public void Cancel(string? remark = null)
    {
        if (TourSchedule.IsStarted())
        {
            throw new AggregateException("Tour schedule is already started");
        }
        
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
        {
            throw new AggregateException($"Can't cancel this tour booking. Status: {Status}.");
        }

        Status = BookingStatus.Cancelled;
        Remark = remark;
        AddOrderCancelDomainEvent();
    }

    public void WaitingForPayment()
    {
        if (Status != BookingStatus.Submitted)
        {
            throw new AggregateException($"Can't payment processing this tour booking. Status: {Status}.");
        }

        Status = BookingStatus.AwaitingPayment;
    }
    
    public void Complete(string? remark = null)
    {
        if (Status != BookingStatus.Paid)
        {
            throw new AggregateException($"Booking wasn't paid. Status: {Status}.");
        }

        Status = BookingStatus.Completed;
        Remark = remark;
        // AddDomainEvent(new OrderCompleted(TourScheduleId));
    }

    public void Purchase(string? remark = null)
    {
        if (Status != BookingStatus.AwaitingPayment)
        {
            throw new AggregateException($"Can't purchase this tour booking. Status: {Status}");
        }

        Status = BookingStatus.Paid;
        Remark = remark;
        AddOrderPaidDomainEvent();
    }

    public bool IsFreeCancellationPeriod()
    {
        var freeCancellationPeriod = CreatedAt.AddDays(1);
        return DateTime.Now < freeCancellationPeriod;
    }

    public bool IsOverdue()
    {
        return CreatedAt.AddHours(1) > DateTime.Now;
    }
    
    private void AddOrderPaidDomainEvent()
    {
        var orderPaid = new OrderPaid
        {
             OrderId = Id,
             OrderCode = Code,
             TourName = TourSchedule.Tour.Title,
             TourDate = TourSchedule.OpenDate,
             FinalCost = NetCost(),
             Email = Email,
             OrderTickets = Tickets.Select(x=> new OrderPaid.OrderedTicket()
             {
                 Code = x.Code,
                 Quantity = x.Quantity,
                 TicketKind = x.TicketType.TicketKind.ToString(),
                 GrossCost = x.GrossCost
             }).ToList()
        };
        
        AddDomainEvent(orderPaid);
    }
    
    private void AddOrderCancelDomainEvent()
    {
        var orderPaid = new OrderCanceled()
        {
            OrderId = Id,
            OrderCode = Code,
            TourName = TourSchedule.Tour.Title,
            TourDate = TourSchedule.OpenDate,
            FinalCost = NetCost(),
            Email = Email,
            OrderTickets = Tickets.Select(x=> new OrderCanceled.OrderedTicket()
            {
                Code = x.Code,
                Quantity = x.Quantity,
                TicketKind = x.TicketType.TicketKind.ToString(),
                GrossCost = x.GrossCost
            }).ToList()
        };
        
        AddDomainEvent(orderPaid);
    }
    public bool IsRated()
    {
        return Rating != null;
    }
    
    public bool CanRatting()
    {
        return IsPaid() && TourSchedule.IsAfterEndDate(1) && IsRated();
    }

    public DateTime OverBookingTime() => CreatedAt.AddHours(1);
    
    public bool IsCancelled() => Status == BookingStatus.Cancelled;
    public bool IsCompleted() => Status == BookingStatus.Completed;
    public bool IsPending() => Status == BookingStatus.AwaitingPayment;
    public bool IsPaid() => Status == BookingStatus.Paid;
    public bool IsPaymentProcessing() => Status == BookingStatus.AwaitingPayment;
}