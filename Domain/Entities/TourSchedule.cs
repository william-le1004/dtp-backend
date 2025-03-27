namespace Domain.Entities;

public class TourSchedule : AuditEntity
{
    public Guid TourId { get; private set; }
    public DateTime OpenDate { get; private set; }

    public DateTime CloseDate { get; private set; }

    public double PriceChangeRate { get; private set; } = 1.0;

    public string? Remark { get; private set; }

    public virtual Tour Tour { get; private set; } = null!;

    private readonly List<TourScheduleTicket> _tourScheduleTickets = new();
    public IReadOnlyCollection<TourScheduleTicket> TourScheduleTickets => _tourScheduleTickets.AsReadOnly();

    public virtual ICollection<TourBooking> TourBookings { get; private set; } = new List<TourBooking>();

    public TourSchedule()
    {
    }
    public TourSchedule(Guid tourId, DateTime startDate, DateTime endDate)
    {
        Id = Guid.NewGuid();
        TourId = tourId;
        OpenDate = startDate;
        CloseDate = endDate;
    }
    public void AddTicket(TourScheduleTicket ticket)
    {
        _tourScheduleTickets.Add(ticket);
    }
    public bool IsAvailable()
    {
        return _tourScheduleTickets.Sum(x => x.AvailableTicket) > 0 && !IsStarted();
    }

    public bool IsBeforeStartDate(int date)
    {
        var beforeStartDate = OpenDate.AddDays(-date);
        return DateTime.Now < beforeStartDate;
    }
    
    public bool IsAvailableTicket(Guid ticketTypeId)
    {
        return _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId).IsAvailable();
    }

    public bool HasAvailableTicket(int quantity, Guid ticketTypeId)
    {
        var tourScheduleTicket = _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId);

        return tourScheduleTicket.HasAvailableTicket(quantity);
    }

    public decimal GetGrossCost(Guid ticketTypeId)
    {
        return _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId).NetCost;
    }

    public bool IsStarted()
    {
        return OpenDate < DateTime.Now;
    }
}