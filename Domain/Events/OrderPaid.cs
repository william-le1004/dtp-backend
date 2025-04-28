using MediatR;

namespace Domain.Events;

public record OrderPaid() : INotification
{
    public Guid OrderId { get; init; }
    public string OrderCode { get; init; }
    public string TourName { get; set; }
    public DateTime? TourDate { get; set; }
    public string Email { get; set; }
    public List<OrderedTicket> OrderTickets { get; set; } = new ();
    public decimal FinalCost { get; set; }
    
    public class OrderedTicket
    {
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal GrossCost { get; set; }
        public string TicketKind { get; set; }
    }
}

