using System.Net;
using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Company.Commands;

public record GrantLicenseCommand(Guid CompanyId,string ConfirmUrl, bool Accept = true)
    : IRequest<ApiResponse<bool>>;

public class GrantLicenseCommandHandler(
    ICompanyRepository companyRepository,
    IEventBus eventBus,
    ILogger<GrantLicenseCommandHandler> logger,
    UserManager<User> userManager,
    IAuthenticationService authenticationService)
    : IRequestHandler<GrantLicenseCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(GrantLicenseCommand request, CancellationToken cancellationToken)
    {
        if (!request.Accept)
        {
            return ApiResponse<bool>.SuccessResult(false, "Company Lack Information To Accept License");
        }

        try
        {
            var company = await companyRepository.GetCompanyAsync(request.CompanyId);
            if (company == null)
            {
                return ApiResponse<bool>.Failure("Company Not Found", (int)HttpStatusCode.NotFound,
                    ["Company Not Found"]);
            }


            var result = await companyRepository.GrantCompanyAsync(company);

            var staff = company.Staffs.FirstOrDefault(u =>
                u.CompanyId == company.Id &&
                userManager.IsInRoleAsync(u, ApplicationRole.OPERATOR).Result);

            if (staff == null)
            {
                return ApiResponse<bool>.Failure("Operator staff not found", (int)HttpStatusCode.NotFound,
                    ["Operator staff not found"]);
            }
            await eventBus.PublishAsync(new UserCreated(
                staff.Name,           
                staff.Email,
                staff.UserName,
                $"{staff.UserName}{ApplicationConst.DefaultPassword}",
                company.Name,
                await authenticationService.GenerateConfirmUrl(staff.Email, request.ConfirmUrl)
            ), cancellationToken);

            logger.LogInformation(
                "Published UserCreated event to queue: Name={Name}, UserName={UserName}, Email={Email}",
                staff.Name,
                staff.UserName,
                staff.Email
            );
            
            return result
                ? ApiResponse<bool>.SuccessResult(true, "Company Already Granted License")
                : ApiResponse<bool>.Failure("Company Not Found", (int)HttpStatusCode.NotFound,
                    ["Company Not Found"]);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", (int)HttpStatusCode.BadRequest, [ex.Message]);
        }
    }
}