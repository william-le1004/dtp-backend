namespace Application.Messaging;

public record EmailConfirmed
{
    public string Name { get; init; }
    public string? Email { get; init; }
    public string ConfirmUrl { get; init; }
}