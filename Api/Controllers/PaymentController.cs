using Api.Extensions;
using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(IHttpContextAccessor httpContextAccessor,
    PayOS payOs, IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult> Checkout(Guid id)
    {
        var result = await mediator.Send(new GetOrderDetail(id));
        var link = await CreatePaymentUri(result.Value);
        
        return result.Match<ActionResult>(
            Some: (value) => Ok(link),
            None: () => NotFound($"Order ({id}) not found."));
    }

    private async Task<CreatePaymentResult> CreatePaymentUri(OrderDetailResponse order)
    {
        var items = new List<ItemData>();
        foreach (var item in order.OrderTickets)
        {
            items.Add(new ItemData(item.TicketKind.ToString(), item.Quantity, (int)item.GrossCost));
        }

        // Get the current request's base URL
        var requestUri = httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{requestUri?.Scheme}://{requestUri?.Host}";

        var paymentData = new PaymentData(
            order.RefCode,
            (int)order.GrossCost,
            $"DTP Payment",
            items,
            $"{baseUrl}/cancel/{order.Code}",
            $"{baseUrl}/success",
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
            var orderCode = body.data.orderCode;
            await mediator.Send(new PayOrder(orderCode));
        }
        return Ok();
    }
}