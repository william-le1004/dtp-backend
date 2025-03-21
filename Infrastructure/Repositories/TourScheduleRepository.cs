using Application.Contracts.Persistence;
using Application.Extensions;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TourScheduleRepository(DtpDbContext context) : ITourScheduleRepository
{
    public Task<IEnumerable<TourSchedule>> GetAllTourSchedulesAsync()
    {
        return (Task<IEnumerable<TourSchedule>>)Task.CompletedTask;
    }

    public async Task<TourSchedule?> GetTourScheduleByIdAsync(Guid tourScheduleId)
    {
        var tourSchedule = await context.TourSchedules
            .Include(x => x.Tour)
            .Include(x => x.TourScheduleTickets)
            .ThenInclude(x=> x.TicketType)
            .AsSplitQuery()
            .AsNoTracking()
            .IsDeleted(false).SingleOrDefaultAsync(t => t.Id == tourScheduleId);

        if (tourSchedule is not null)
        {
            return MapAvailableTourScheduleTickets(tourSchedule);
        }

        return null;
    }

    public Task<IEnumerable<TourSchedule>> GetTourSchedulesByTourIdAsync(Guid tourId)
    {
        return (Task<IEnumerable<TourSchedule>>)Task.CompletedTask;
    }

    public async Task<IEnumerable<TourSchedule>> GetTourSchedulesByRange(List<Guid> tourScheduleIds)
    {
        var tourSchedule = await context.TourSchedules
            .Include(x => x.Tour)
            .Include(x => x.TourScheduleTickets)
            .ThenInclude(x => x.TicketType)
            .AsSplitQuery()
            .AsNoTracking()
            .IsDeleted(false).Where(t => tourScheduleIds.Contains(t.Id)).ToListAsync();

        return tourSchedule.Select(MapAvailableTourScheduleTickets);
    }

    private TourSchedule MapAvailableTourScheduleTickets(TourSchedule tourSchedule)
    {
        var tourScheduleTicket = tourSchedule.TourScheduleTickets;

        var tickets = context.TourBookings.Include(x => x.Tickets)
            .Where(b => b.TourScheduleId == tourSchedule.Id && b.Status != BookingStatus.Cancelled)
            .SelectMany(x => x.Tickets).ToList();

        _ = tourScheduleTicket.GroupJoin(tickets,
                t => t.TicketTypeId,
                o => o.TicketTypeId,
                (dailyTicket, orderedTicket) => new
                {
                    DailyTicket = dailyTicket,
                    OrderedTicket = orderedTicket.Sum(ticket => ticket.Quantity)
                })
            .Select(x =>
            {
                x.DailyTicket.CalAvailableTicket(x.OrderedTicket);
                return x.DailyTicket;
            }).ToList();

        return tourSchedule;
    }
}