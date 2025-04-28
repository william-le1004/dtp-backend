namespace Application.Messaging.Order;

public record OrderPaidIntegrationEvent
{
    public Guid OrderId { get; init; }
    public string OrderCode { get; init; }
    public string TourName { get; set; }
    
    public string Email { get; set; }
    public DateTime? TourDate { get; set; }
    public List<OrderTicketIntegrationEvent> OrderTickets { get; set; } = new ();
    public decimal FinalCost { get; set; }
}

public record OrderTicketIntegrationEvent
{
    public string Code { get; set; }
    public int Quantity { get; set; }
    public decimal GrossCost { get; set; }
    public string TicketKind { get; set; }
}