namespace Application.Dtos;

public record LoginRequestDto(string UserNameOrPassword, string Password, string FcmToken);