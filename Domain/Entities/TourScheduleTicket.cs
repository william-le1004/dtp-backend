namespace Domain.Entities;

public class TourScheduleTicket
{
    public decimal NetCost { get; set; }
    public int AvailableTicket { get; set; }
    public Guid TicketTypeId { get; set; }
    public TicketType TicketType { get; set; } = null!;
    public Guid TourScheduleId { get; set; }
    public TourSchedule TourSchedule { get; set; } = null!;

    public bool IsAvailable() => AvailableTicket > 0;
    public bool HasAvailableTicket(int quantity) => AvailableTicket > quantity;

}