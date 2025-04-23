using System.Net;
using Application.Common;
using Application.Contracts.Authentication;
using Application.Contracts.Caching;
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
    IAuthenticationService authenticationService,
    IRedisCacheService redisCache)
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
            const string cacheKey = "GetAllCompanies";
            var result = await companyRepository.GrantCompanyAsync(request.CompanyId);
            var staff = result.Staffs.FirstOrDefault(u =>
                u.CompanyId == result.Id &&
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
                result.Name,
                await authenticationService.GenerateConfirmUrl(staff.Email, request.ConfirmUrl)
            ), cancellationToken);

            logger.LogInformation(
                "Published UserCreated event to queue: Name={Name}, UserName={UserName}, Email={Email}",
                staff.Name,
                staff.UserName,
                staff.Email
            );

            await redisCache.RemoveDataAsync(cacheKey);
            return ApiResponse<bool>.SuccessResult(true, "Company Already Granted License");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failure($"An error occurred", (int)HttpStatusCode.BadRequest, [ex.Message]);
        }
    }
}