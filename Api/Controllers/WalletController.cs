using Api.Filters;
using Application.Features.Wallet.Commands;
using Application.Features.Wallet.Queries;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController(IMediator mediator) : ODataController
{
    [HttpPost("otp/config")]
    public async Task<ActionResult> OtpConfig(OtpUserConfig request)
    {
        var imageUrl = await mediator.Send(request);
        return imageUrl is null ? BadRequest() : Ok(imageUrl);
    }

    [Authorize]
    [HttpPost("otp")]
    public async Task<IActionResult> GetOtpConfig(OtpUserRequest request)
    {
        var result = await mediator.Send(request);
        return result ? Ok() : NotFound();
    }
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<WalletDetailsResponse>> GetWalletDetails()
    {
        var result = await mediator.Send(new GetOwnWallet());

        return result.Match<ActionResult<WalletDetailsResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Wallet not found."));
    }

    [EnableQuery]
    [HttpGet("transaction")]
    [Authorize]
    public async Task<IQueryable<TransactionResponse>> Get()
    {
        return await mediator.Send(new GetTransactionHistory());
    }
    
    [EnableQuery]
    [HttpGet("external-transaction")]
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<IQueryable<ExternalTransactionResponse>> ExternalTransaction()
    {
        return await mediator.Send(new GetExternalTransaction());
    }
    
    [HttpPost("external-transaction/{id}/accept")]
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<IActionResult> AcceptExternalTransaction(Guid id)
    {
        await mediator.Send(new AcceptExternalTransaction(id));
        return NoContent();
    }
    
    [EnableQuery]
    [HttpGet("own-external-transaction")]
    [Authorize]
    public async Task<IQueryable<ExternalTransactionResponse>> OwnExternalTransaction()
    {
        return await mediator.Send(new GetOwnExternalTransaction());
    }

    [HttpGet("transaction/{id}")]
    [Authorize]
    public async Task<ActionResult<TransactionDetailResponse>> GetWalletTransactionDetails(Guid id)
    {
        var result = await mediator.Send(new GetTransactionDetails(id));

        return result.Match<ActionResult<TransactionDetailResponse>>(
            Some: (value) => Ok(value),
            None: () => NotFound($"Wallet not found."));
    }

    [ServiceFilter(typeof(OtpAuthorizeFilter))]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(WalletDeposit request)
    {
        return Created();
    }

    [ServiceFilter(typeof(OtpAuthorizeFilter))]
    [HttpPost("withdraw")]
    [Authorize]
    public async Task<IActionResult> Withdraw(WalletWithdraw request)
    {
        var withdrawRequest = await mediator.Send(request);
        
        return withdrawRequest.Match<ActionResult>(
            Some: (value) => Ok(new {id = value.Id}),
            None: () => BadRequest($"Wallet not found."));
    }
    
    [ServiceFilter(typeof(OtpAuthorizeFilter))]
    [HttpPost("tranfer")]
    public async Task<IActionResult> Transfer(WalletTransfer request)
    {
        return Created();
    }
}