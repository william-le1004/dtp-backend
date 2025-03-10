using Application.Features.Company.Commands;
using Application.Features.Company.Queries;
using Infrastructure.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = ApplicationConst.AUTH_POLICY)]
public class CompanyController : BaseController
{
    private readonly IMediator _mediator;

    public CompanyController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Authorize(Policy = ApplicationConst.ADMIN_POLICY)]
    public async Task<IActionResult> Get()
    {
        var response = await _mediator.Send(new GetCompaniesQuery());
        return HandleServiceResult(response);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromQuery] Guid id)
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
    public async Task<IActionResult> Update([FromQuery] string id)
    {
        var response = await _mediator.Send(new DeleteCompanyCommand(id));
        return HandleServiceResult(response);
    }
}