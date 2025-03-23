using Api.Filters;
using Application.Features.Wallet.Commands;
using Application.Features.Wallet.Queries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController(IMediator mediator) : ODataController
{
    [HttpGet("otp")]
    public async Task<ActionResult> OtpConfig()
    {
        var imageUrl = await mediator.Send(new OtpUserConfig());
        return Ok(imageUrl);
    }

    [HttpGet]
    public async Task<ActionResult<WalletDetailsResponse>> GetWalletDetails()
    {
        var result = await mediator.Send(new GetOwnWallet());

        return result.Match<ActionResult<WalletDetailsResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Wallet not found."));
    }

    [EnableQuery]
    [HttpGet("transaction")]
    public async Task<IQueryable<TransactionResponse>> GetWalletTransaction()
    {
        return await mediator.Send(new GetTransactionHistory());
    }

    [HttpGet("transaction/{id}")]
    public async Task<ActionResult<TransactionDetailResponse>> GetWalletTransactionDetails(Guid id)
    {
        var result = await mediator.Send(new GetTransactionDetails(id));

        return result.Match<ActionResult<TransactionDetailResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Wallet not found."));
    }

    [OtpAuthorize]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit()
    {
        return Created();
    }

    [OtpAuthorize]
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw()
    {
        return Created();
    }
}