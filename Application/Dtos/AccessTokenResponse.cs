namespace Application.Dtos;

public class AccessTokenResponse
{
    public string TokenType { get; } = "Bearer";

    public string AccessToken { get; init; }

    public long ExpiresIn { get; init; }

    public string RefreshToken { get; init; }

    public string Role { get; init; }
}