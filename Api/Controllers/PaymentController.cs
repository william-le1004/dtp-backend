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
        var result = await mediator.Send(request);
        
        return result.Match<ActionResult>(
            Some: Ok,
            None: () => NotFound($"Order ({request.BookingId}) not found."));
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