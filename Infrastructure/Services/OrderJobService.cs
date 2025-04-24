using Application.Contracts.Job;
using Domain.Constants;
using Domain.Enum;
using Hangfire;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class OrderJobService(
    IBackgroundJobClient jobClient,
    DtpDbContext context,
    ILogger<OrderJobService> logger
) : IOrderJobService
{
    public void ScheduleCancelOrder(Guid bookingId)
    {
        jobClient.Schedule(() => CancelOrder(bookingId), TimeSpan.FromHours(1));
    }

    public void PaidCheck(string jobId)
    {
        jobClient.Delete(jobId);
        logger.LogInformation("Deleted cancel order job when paid {jobId}", jobId);
    }

    [Queue(ApplicationConst.CancelOrderQueue)]
    public async Task CancelOrder(Guid bookingId)
    {
        var order = await context.TourBookings
            .Include(x => x.Tickets)
            .ThenInclude(x => x.TicketType)
            .Include(x => x.TourSchedule)
            .ThenInclude(x=> x.Tour)
            .AsSingleQuery()
            .FirstOrDefaultAsync(x => x.Id == bookingId && x.Status == BookingStatus.AwaitingPayment);
        if (order is not null)
        {
            var payment = context.Payments
                .FirstOrDefault(x=> x.BookingId == bookingId && x.Status == PaymentStatus.Pending);
            if (payment is not null)
            {
                payment.Cancel();
            }
            order.Cancel();
            logger.LogInformation("Cancelling order by system {bookingId}", bookingId);
            await context.SaveChangesAsync();
        }
    }

    [Queue(ApplicationConst.CompleteOrderQueue)]
    public async Task MarkToursAsCompleted()
    {
        var orders = await context.TourBookings
            .Include(x => x.TourSchedule)
            .Where(x => x.Status == BookingStatus.Paid
                        && x.TourSchedule.CloseDate != null
                        && x.TourSchedule.CloseDate.Value.AddDays(4) >= DateTime.Now)
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Complete();
        }
        
        context.TourBookings.UpdateRange(orders);
        await context.SaveChangesAsync();
    }


    [Queue(ApplicationConst.CompleteOrderQueue)]
    public async Task MarkOrderCompleted(Guid bookingId)
    {
        var order = await context.TourBookings
            .Include(x => x.TourSchedule)
            .FirstOrDefaultAsync(x => x.Status == BookingStatus.Paid
                                      && x.Id == bookingId);

        if (order is not null)
        {
            order.Complete();
            context.TourBookings.Update(order);
            await context.SaveChangesAsync();
        }
    }
    
    public void ScheduleCompleteOrder(Guid bookingId)
    {
        jobClient.Schedule(() => MarkOrderCompleted(bookingId), TimeSpan.FromMinutes(1));
    }
}