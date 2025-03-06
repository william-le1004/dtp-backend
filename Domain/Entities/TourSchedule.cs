namespace Domain.Entities;

public class TourSchedule : AuditEntity
{
    public Guid TourId { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public double PriceChangeRate { get; set; } = 1.0;

    public string? Remark { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public List<TourScheduleTicket> TourScheduleTickets = new();

    public virtual ICollection<TourBooking> TourBookings { get; private set; } = new List<TourBooking>();

    public TourSchedule()
    {
    }

    // public void AddTicket(TourScheduleTicket ticket)
    // {
    //     // Giả sử backing field tourScheduleTickets là List<TourScheduleTicket>
    //     _tourScheduleTickets.Add(ticket);
    // }
    // public bool IsAvailable()
    // {
    //     return _tourScheduleTickets.Sum(x => x.AvailableTicket) > 0 && !IsStarted();
    // }
    //
    // public bool IsAvailableTicket(Guid ticketTypeId)
    // {
    //     return _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId).IsAvailable();
    // }
    //
    // public bool HasAvailableTicket(int quantity, Guid ticketTypeId)
    // {
    //     var tourScheduleTicket = _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId);
    //
    //     return tourScheduleTicket.HasAvailableTicket(quantity);
    // }
    //
    // public decimal GetGrossCost(Guid ticketTypeId)
    // {
    //     return _tourScheduleTickets.Single(x => x.TicketTypeId == ticketTypeId).NetCost;
    // }

    public bool IsStarted()
    {
        return StartDate < DateTime.Now;
    }
}