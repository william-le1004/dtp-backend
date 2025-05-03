using Application.Contracts.Persistence;
using Application.Features.Voucher.Commands;
using Application.Features.Voucher.Queries;
using Domain.Constants;
using Domain.ValueObject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class VoucherController(IMediator mediator, IVoucherRepository repository) : ODataController
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

    [HttpGet("{code}")]
    public async Task<ActionResult<Voucher>> GetVoucherByCode(string code)
    {
        var voucher = await repository.GetVoucherByCodeAsync(code);

        if (voucher is null)
        {
            return NotFound();
        }
        return voucher;
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<ActionResult> PutVoucher(Guid id, UpdateVoucherCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = ApplicationRole.ADMIN)]
    public async Task<ActionResult> DeleteVoucher(Guid id)
    {
        var result = await mediator.Send(new DeleteVoucherCommand(id));
        
        return result.Match<ActionResult>(
            Some: (value) => NoContent(),
            None: () => NotFound($"Voucher ({id}) not found."));
    }
}