namespace Domain.Entities;

public class TourScheduleTicket
{
    public decimal NetCost { get; private set; }
    public double Tax { get; private set; } = 0.1;
    public int AvailableTicket { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public TicketType TicketType { get; private set; } = null!;
    public Guid TourScheduleId { get; private set; }
    public TourSchedule TourSchedule { get; private set; } = null!;

    public decimal GrossCost
    {
        get
        {
            return NetCost + ((decimal)Tax * NetCost);
        }
    }

    public bool IsAvailable() => AvailableTicket > 0;
    public bool HasAvailableTicket(int quantity) => AvailableTicket > quantity;
    public TourScheduleTicket(Guid id, decimal netCost, double tax, int availableTicket, Guid ticketTypeId, Guid tourScheduleId)
    {
        Id = id;
        NetCost = netCost;
        Tax = tax;
        AvailableTicket = availableTicket;
        TicketTypeId = ticketTypeId;
        TourScheduleId = tourScheduleId;
    }

}