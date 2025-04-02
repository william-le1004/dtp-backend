namespace Application.Contracts.Job;

public interface IHangfireJobService
{
    Task HardDeleteExpiredEntities();
}