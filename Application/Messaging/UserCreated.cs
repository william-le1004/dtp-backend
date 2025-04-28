namespace Application.Messaging;

public record UserCreated(
    string Name,
    string Email,
    string UserName,
    string DefaultPassword,
    string CompanyName,
    string ConfirmUrl);