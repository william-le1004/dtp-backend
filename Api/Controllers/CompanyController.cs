using Application.Dtos;
using Application.Features.Company.Commands;
using Application.Features.Company.Queries;
using Application.Features.Tour.Queries;
using Domain.Constants;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyController(IMediator mediator) : BaseController
{
    [HttpGet]
    [Authorize(Policy = ApplicationConst.AdminPermission)]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var response = await mediator.Send(new GetCompaniesQuery());
        return ReturnList(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var response = await mediator.Send(new GetCompanyQuery(id));
        return HandleServiceResult(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpPut]
    [Authorize(Policy = ApplicationConst.AuthenticatedUser)]
    public async Task<IActionResult> Update([FromBody] UpdateCompanyCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = ApplicationConst.AuthenticatedUser)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var response = await mediator.Send(new DeleteCompanyCommand(id));
        return HandleServiceResult(response);
    }

    [HttpPut("grant")]
    [Authorize(Policy = ApplicationConst.AdminPermission)]
    public async Task<IActionResult> Grant(GrantLicenseCommand command)
    {
        var response = await mediator.Send(command);
        return HandleServiceResult(response);
    }
    
    [HttpGet("tour")]
    [EnableQuery]
    public async Task<IQueryable<TourByCompanyResponse>> Tour()
    {
        return await mediator.Send(new GetListTourByCompanyQuery());
    }
}