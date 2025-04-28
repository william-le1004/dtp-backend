namespace Application.Messaging;

public record PasswordForget(
    string Email,
    string ConfirmUrl);
