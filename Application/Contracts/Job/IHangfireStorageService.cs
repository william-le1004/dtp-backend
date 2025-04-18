namespace Application.Contracts.Job;

public interface IHangfireStorageService
{
    string GetScheduleJobIdByArgId(string methodName, Guid argId, int? from = null,
        int? count = null);
}