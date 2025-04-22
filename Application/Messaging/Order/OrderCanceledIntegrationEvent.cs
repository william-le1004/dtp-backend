namespace Application.Messaging.Order;

public record OrderCanceledIntegrationEvent
{
    public Guid OrderId { get; init; }
    public string OrderCode { get; init; }
    public string TourName { get; set; }
    
    public string Email { get; set; }
    public DateTime? TourDate { get; set; }
    public List<OrderTicketIntegrationEvent> OrderTickets { get; set; } = new ();
    public decimal FinalCost { get; set; }
}