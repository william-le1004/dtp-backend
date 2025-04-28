namespace Application.Dtos;

public record RegistrationRequestDto(
    string Name,
    string Address,
    string Email,
    string UserName,
    string PhoneNumber,
    string Password,
    string ConfirmUrl);