using System.Text.Json.Serialization;
using Application.Contracts;
using Application.Contracts.Persistence;
using Domain.Entities;
using Domain.Enum;
using Functional.Option;
using PaymentEntity = Domain.Entities.Payment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;

namespace Application.Features.Payment.Commands;

public record UriResponse(string ReturnUrl, string CancelUrl);

public record PaymentProcessor : IRequest<Option<string>>
{
    public Guid BookingId { get; init; }
    public required UriResponse ResponseUrl { get; init; }

    [JsonIgnore] public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.PayOs;
}

public class PaymentProcessorHandler(IDtpDbContext context, IUserContextService service, PayOS payOs)
    : IRequestHandler<PaymentProcessor, Option<string>>
{
    public async Task<Option<string>> Handle(PaymentProcessor request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;

        var order = await context.TourBookings
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.BookingId && x.UserId == userId,
                cancellationToken: cancellationToken);

        if (order is not null)
        {
            var link = await CreatePaymentUri(order, request.ResponseUrl);
            var payment = new PaymentEntity(order.Id, link.paymentLinkId, link.amount);
            order.WaitingForPayment();
            context.Payments.Add(payment);
            context.TourBookings.Update(order);
            await context.SaveChangesAsync(cancellationToken);

            return link.checkoutUrl;
        }

        return Option.None;
    }

    private async Task<CreatePaymentResult> CreatePaymentUri(TourBooking order, UriResponse uriResponse)
    {
        var items = new List<ItemData>();
        foreach (var item in order.Tickets)
        {
            items.Add(new ItemData(item.TicketType.TicketKind.ToString(), item.Quantity, (int)item.GrossCost));
        }

        var paymentData = new PaymentData(
            order.RefCode,
            (int)order.NetCost(),
            $"DTP Payment",
            items,
            uriResponse.CancelUrl,
            uriResponse.ReturnUrl,
            null,
            order.Name,
            order.Email,
            order.PhoneNumber,
            null,
            ((DateTimeOffset)order.OverBookingTime()).ToUnixTimeSeconds()
        );

        var createPayment = await payOs.createPaymentLink(paymentData);
        return createPayment;
    }
}