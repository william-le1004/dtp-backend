using Application.Contracts;
using Application.Contracts.Authentication;
using Application.Contracts.Cloudinary;
using Application.Contracts.Persistence;
using Domain.DataModel;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Wallet.Commands;

public record OtpUserConfig() : IRequest<string?>
{
    public string ConfirmationToken { get; set; }
}

public class OtpUserConfigHandler(
    UserManager<User> manager,
    IAuthenticatorService service,
    IDtpDbContext context,
    ICloudinaryService cloudinaryService
) : IRequestHandler<OtpUserConfig, string?>
{
    public async Task<string?> Handle(OtpUserConfig request, CancellationToken cancellationToken)
    {
        var user = await manager.Users
            .Where(u => u.SecureToken == request.ConfirmationToken)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
        {
            return null;
        }

        var key = service.GenerateSecretKey();
        user.OtpKey = key;
        await manager.UpdateAsync(user);

        var qrByte = service.GenerateQrCode(key, user.Email);
        var imageUrl = await cloudinaryService.UploadImageAsync(new MemoryStream(qrByte!), user.Id);
        SaveQrUrl(imageUrl, user.Id, cancellationToken);
        
        return imageUrl;
    }

    private void SaveQrUrl(string url, string userId,CancellationToken cancellationToken )
    {
        Guid.TryParse(userId, out Guid userGuidId);
        
        context.ImageUrls.Add(new ImageUrl(userGuidId, url));

        context.SaveChangesAsync(cancellationToken);
    }
    
}