using Application.Features.Company.Commands;
using Application.Features.Company.Queries;
using Domain.Constants;
using Infrastructure.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = ApplicationConst.AuthenticatedUser)]
public class CompanyController : BaseController
{
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = ApplicationConst.AdminPermission)]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetCompaniesQuery());
        return ReturnList(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetCompanyQuery(id));
        return HandleServiceResult(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
    {
        var response = await _mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyCommand command)
    {
        var response = await _mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new DeleteCompanyCommand(id));
        return HandleServiceResult(response);
    }

    [HttpPut("grant")]
    [Authorize(Policy = ApplicationConst.AdminPermission)]
    public async Task<IActionResult> Grant(GrantLicenseCommand command)
    {
        var response = await _mediator.Send(command);
        return HandleServiceResult(response);
    }
}