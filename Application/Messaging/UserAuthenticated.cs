namespace Application.Messaging;

public record UserAuthenticated(
    string Name,
    string UserName,
    string Email
);