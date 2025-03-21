using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface ITourScheduleRepository
{
    Task<IEnumerable<TourSchedule>> GetAllTourSchedulesAsync();
    Task<TourSchedule?> GetTourScheduleByIdAsync(Guid tourScheduleId);
    Task<IEnumerable<TourSchedule>> GetTourSchedulesByTourIdAsync(Guid tourId);
    Task<IEnumerable<TourSchedule>> GetTourSchedulesByRange(List<Guid> tourScheduleIds);
}