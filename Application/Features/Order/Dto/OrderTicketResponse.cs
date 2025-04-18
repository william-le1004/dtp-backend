using Domain.Enum;

namespace Application.Features.Order.Dto;

public record OrderTicketResponse
{
    public string Code { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal GrossCost { get; set; }
    public TicketKind TicketKind { get; set; }
}