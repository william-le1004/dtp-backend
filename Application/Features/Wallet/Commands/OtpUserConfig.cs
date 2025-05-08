using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Cloudinary;
using Application.Contracts.Persistence;
using Domain.DataModel;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Wallet.Commands;

public record OtpUserConfig() : IRequest<string>;

public class OtpUserConfigHandler(
    UserManager<User> manager,
    IAuthenticatorService service,
    IDtpDbContext context,
    IUserContextService userService,
    ICloudinaryService cloudinaryService
) : IRequestHandler<OtpUserConfig, string>
{
    public async Task<string> Handle(OtpUserConfig request, CancellationToken cancellationToken)
    {
        var userId = userService.GetCurrentUserId()!;

        var user = manager.FindByIdAsync(userId).GetAwaiter().GetResult();

        if (user is null)
        {
            return null;
        }

        var key = service.GenerateSecretKey();
        user.OtpKey = key;
        await manager.UpdateAsync(user);

        var qrByte = service.GenerateQrCode(key, user.Email);
        var imageUrl = await cloudinaryService.UploadImageAsync(new MemoryStream(qrByte!), userId);
        SaveQrUrl(imageUrl, userId, cancellationToken);
        
        return imageUrl;
    }

    private void SaveQrUrl(string url, string userId,CancellationToken cancellationToken )
    {
        Guid.TryParse(userId, out Guid userGuidId);
        
        context.ImageUrls.Add(new ImageUrl(userGuidId, url));

        context.SaveChangesAsync(cancellationToken);
    }
    
}