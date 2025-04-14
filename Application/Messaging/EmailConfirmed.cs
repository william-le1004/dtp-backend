namespace Application.Messaging;

public record EmailConfirmed(
    string Name,
    string Email,
    string ConfirmUrl);