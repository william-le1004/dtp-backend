using Domain.Enum;

namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get; set; }
    public decimal NetCost { get; set; }
    public double Tax { get; set; } = 0.1;
    public TicketKind TicketKind { get; set; }
    public Guid TourScheduleId { get; set; }
}