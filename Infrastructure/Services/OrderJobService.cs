using Application.Contracts.Job;
using Domain.Constants;
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
            .Include(x => x.TourSchedule)
            .FirstOrDefaultAsync(x => x.Id == bookingId);
        if (order is not null && order.IsPending() && order.IsOverdue())
        {
            order.Cancel();
            logger.LogInformation("Cancelling order by system {bookingId}", bookingId);
            await context.SaveChangesAsync();
        }
    }
}