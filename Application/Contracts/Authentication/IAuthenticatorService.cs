namespace Application.Contracts.Authentication;

public interface IAuthenticatorService
{
    public string GenerateSecretKey();
    public byte[] GenerateQrCode(string secretKey, string userEmail);
}