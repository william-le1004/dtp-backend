namespace Domain.Entities;

public class Cart
{
    public Guid UserId { get; set; }

    public Guid TourScheduleId { get; set; }

    public int Quantity { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}