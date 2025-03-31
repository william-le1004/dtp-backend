namespace Application.Events;

public record UserCreated
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
}