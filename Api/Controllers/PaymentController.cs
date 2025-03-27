using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Application.Features.Payment.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(
    PayOS payOs, IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Checkout(PaymentProcessor request)
    {
        var result = await mediator.Send(new GetOrderDetail(request.BookingId));
        var link = await CreatePaymentUri(result.Value, request.ResponseUrl);
        request.PaymentLinkId = link.paymentLinkId;
        request.NetCost = link.amount;
        
        await mediator.Send(request);
        
        return result.Match<ActionResult>(
            Some: (value) => Ok(link),
            None: () => NotFound($"Order ({request.BookingId}) not found."));
    }

    private async Task<CreatePaymentResult> CreatePaymentUri(OrderDetailResponse order, UriResponse uriResponse)
    {
        var items = new List<ItemData>();
        foreach (var item in order.OrderTickets)
        {
            items.Add(new ItemData(item.TicketKind.ToString(), item.Quantity, (int)item.GrossCost));
        }

        var paymentData = new PaymentData(
            order.RefCode,
            (int)order.NetCost,
            $"DTP Payment",
            items,
            uriResponse.CancelUrl,
            uriResponse.ReturnUrl,
            null,
            order.Name,
            order.Email,
            order.PhoneNumber
        );

        var createPayment = await payOs.createPaymentLink(paymentData);
        return createPayment;
    }
    
    [HttpPost("callback")]
    public async Task<IActionResult> PayOsTransferHandler(WebhookType body)
    {
        try
        {
            _ = payOs.verifyPaymentWebhookData(body);
        }
        catch (Exception)
        {
            return NoContent();
        }

        if (body.success)
        {
            var data = body.data;
            await mediator.Send(new PayOrder(data.orderCode, data.amount, data.reference));
        }
        return Ok();
    }
}