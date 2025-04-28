using Domain.Enum;

namespace Domain.Entities;

public class TicketType
{
    public Guid Id { get; private set; }
    public decimal DefaultNetCost { get; private set; }
    public int MinimumPurchaseQuantity { get; private set; }
    public TicketKind TicketKind { get; private set; }
    public Guid TourId { get; private set; }

    public TicketType(decimal defaultNetCost, int minimumPurchaseQuantity, TicketKind ticketKind, Guid tourId)
    {
        Id = Guid.NewGuid();
        DefaultNetCost = defaultNetCost;
        MinimumPurchaseQuantity = minimumPurchaseQuantity;
        TicketKind = ticketKind;
        TourId = tourId;
    }
}