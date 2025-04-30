using Application.Features.Voucher.Commands;
using Application.Features.Voucher.Queries;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class VoucherController(IMediator mediator) : ODataController
{
    [HttpGet]
    [EnableQuery]
    public Task<IQueryable<VoucherResponse>> Get()
    {
        return mediator.Send(new GetVouchers());
    }
    
    [HttpPost]
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<ActionResult> PostVoucher(CreateVoucherCommand command)
    {
        var voucherId = await mediator.Send(command);
        return Ok( new { id = voucherId });
    }
}