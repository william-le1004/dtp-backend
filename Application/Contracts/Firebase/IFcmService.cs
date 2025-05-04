namespace Application.Contracts.Firebase;

public interface IFcmService
{
    Task SendNotificationAsync(string token, string title, string body);
}