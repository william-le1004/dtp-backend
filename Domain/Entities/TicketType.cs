using Domain.Enum;

namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get; private set; }
    public decimal DefaultNetCost { get; private set; }
    public double DefaultTax { get; private set; } = 0.1;
    public int MinimumPurchaseQuantity { get; private set; }
    public TicketKind TicketKind { get; private set; }
    public Guid TourId { get; private set; }
}