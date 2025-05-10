namespace Application.Contracts.Firebase;

public interface IFcmService
{
    Task SendNotificationAsync(string title, string body);
}