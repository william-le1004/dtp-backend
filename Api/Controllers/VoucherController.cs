using Application.Features.Voucher.Queries;
using MediatR;
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
}