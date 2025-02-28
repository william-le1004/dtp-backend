namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal GrossCost { get; set; }
    public Guid TourBookingId { get; set; }
    public virtual TourBooking TourBooking { get; set; }
}