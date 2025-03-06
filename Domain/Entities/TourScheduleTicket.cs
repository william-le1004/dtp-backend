namespace Domain.Entities;

public class TourScheduleTicket
{
    public decimal NetCost { get; private set; }
    public int AvailableTicket { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public TicketType TicketType { get; private set; } = null!;
    public Guid TourScheduleId { get; private set; }
    public TourSchedule TourSchedule { get; private set; } = null!;

    public bool IsAvailable() => AvailableTicket > 0;
    public bool HasAvailableTicket(int quantity) => AvailableTicket > quantity;
    public TourScheduleTicket(decimal netCost, int availableTicket, Guid ticketTypeId, Guid tourScheduleId)
    {
        NetCost = netCost;
        AvailableTicket = availableTicket;
        TicketTypeId = ticketTypeId;
        TourScheduleId = tourScheduleId;
    }

}