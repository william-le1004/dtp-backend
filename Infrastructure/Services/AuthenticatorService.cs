using Application.Contracts.Authentication;
using Domain.Constants;
using Infrastructure.Common.Constants;
using OtpNet;
using QRCoder;

namespace Infrastructure.Services;

public class AuthenticatorService : IAuthenticatorService
{
    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    private string GenerateQrCodeUri(string secretKey, string userEmail)
    {
        string issuer = ApplicationConst.AppName;
        string accountName = userEmail;

        string otpUri = $"otpauth://totp/{issuer}:{accountName}?secret={secretKey}&issuer={issuer}";

        return otpUri;
    }

    public byte[] GenerateQrCode(string secretKey, string userEmail)
    {
        var otpUri = GenerateQrCodeUri(secretKey, userEmail);
        
        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20);
            }
        }
    }
}