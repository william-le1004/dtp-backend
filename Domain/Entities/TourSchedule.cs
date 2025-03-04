namespace Domain.Entities;

public class TourSchedule : AuditEntity
{
    public Guid TourId { get; private set; }
    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public double PriceChangeRate { get; private set; } = 1.0;

    public string? Remark { get; private set; }

    public virtual Tour Tour { get; private set; } = null!;

    private readonly List<TourScheduleTicket> tourScheduleTickets = new();
    public IReadOnlyCollection<TourScheduleTicket> TourScheduleTickets => tourScheduleTickets.AsReadOnly();

    public virtual ICollection<TourBooking> TourBookings { get; private set; } = new List<TourBooking>();

    public bool IsAvailable()
    {
        return tourScheduleTickets.Sum(x => x.AvailableTicket) > 0 && !IsStarted();
    }
    
    public bool IsAvailableTicket(Guid tourScheduleTicketId)
    {
        return tourScheduleTickets.Single(x => x.Id == tourScheduleTicketId).IsAvailable();
    }

    public bool HasAvailableTicket(int quantity, Guid tourScheduleTicketId)
    {
        var tourScheduleTicket = tourScheduleTickets.Single(x => x.Id == tourScheduleTicketId);

        return tourScheduleTicket.HasAvailableTicket(quantity);
    }

    public decimal GetGrossCost(Guid tourScheduleTicketId)
    {
        return tourScheduleTickets.Single(x => x.Id == tourScheduleTicketId).GrossCost;
    }

    public bool IsStarted()
    {
        return StartDate > DateTime.Now;
    }
}