using Domain.Enum;

namespace Application.Features.Order.Dto;

public record OrderDetailResponse
{
    public Guid Id { get; init; }
    public Guid TourId { get; init; }
    public string Code { get; set; }

    public long RefCode { get; set; }
    public string Name { get; set; }

    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string TourName { get; set; }
    public string? TourThumbnail { get; set; }
    public Guid TourScheduleId { get; set; }
    public DateTime TourDate { get; set; }

    public DateTime OrderDate { get; set; }

    public IEnumerable<OrderTicketResponse> OrderTickets { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal GrossCost { get; set; }
    public decimal NetCost { get; set; }

    public BookingStatus Status { get; set; }

    public string? PaymentLinkId { get; init; }

    public PaymentStatus? PaymentStatus { get; set; }
};