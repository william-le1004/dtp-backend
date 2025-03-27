using System.Text.Json.Serialization;
using Application.Contracts.Persistence;
using Domain.Enum;
using PaymentEntity = Domain.Entities.Payment;
using MediatR;

namespace Application.Features.Payment.Commands;

public record UriResponse(string ReturnUrl, string CancelUrl);

public record PaymentProcessor : IRequest
{
    public Guid BookingId { get; init; }
    
    [JsonIgnore]
    public string? PaymentLinkId { get; set; }
    public UriResponse ResponseUrl { get; set; }

    [JsonIgnore] public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.PayOs;
}

public class PaymentProcessorHandler(IDtpDbContext context) : IRequestHandler<PaymentProcessor>
{
    public async Task Handle(PaymentProcessor request, CancellationToken cancellationToken)
    {
        var payment = new PaymentEntity(request.BookingId, request.PaymentLinkId);
        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);
    }
}