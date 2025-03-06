using Domain.Enum;

namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get;  set; }
    public decimal DefaultNetCost { get;  set; }
    public int MinimumPurchaseQuantity { get;  set; }
    public TicketKind TicketKind { get;  set; }
    public Guid TourId { get;  set; }

 
}