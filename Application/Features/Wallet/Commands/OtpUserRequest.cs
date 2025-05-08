using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Wallet.Commands;

public record OtpUserRequest :IRequest<bool>
{
    public string ConfirmUrl { get; set; }
}

public class OtpUserRequestHandler(IUserContextService service,
    IUserRepository repository,
    IAuthenticationService authenticationService,
    IEventBus eventBus,
    ILogger<OtpUserRequestHandler> logger) : IRequestHandler<OtpUserRequest, bool>
{
    public async Task<bool> Handle(OtpUserRequest request, CancellationToken cancellationToken)
    {
        var userId = service.GetCurrentUserId()!;
        var user = await repository.GetUserDetailAsync(userId);
        if (user is not null)
        {
            
            await eventBus.PublishAsync(
                new EmailConfirmed(
                    user.Name,
                    user.Email!,
                    await authenticationService.GenerateConfirmUrl(user.Email!, request.ConfirmUrl, false)
                ),
                cancellationToken
            );
            logger.LogInformation($"OTP {userId} was successfully mailing");
            return true;
        }
        return false;
    }
}