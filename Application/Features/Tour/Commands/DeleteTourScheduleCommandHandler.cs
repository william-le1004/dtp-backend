using Application.Common;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Features.Wallet.Events;
using Application.Messaging.Tour;
using Application.Messaging.Wallet;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // Command nhận vào TourId và khoảng thời gian cần xóa lịch trình
    public record DeleteTourSchedule(
        Guid TourId,
        DateOnly StartDay,
        DateOnly EndDay,
        string? Remark
    ) : IRequest<ApiResponse<string>>;

    public class DeleteTourScheduleHandler : IRequestHandler<DeleteTourSchedule, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;
        private readonly IPublisher _publisher;
        IEventBus eventBus;
        public DeleteTourScheduleHandler(IDtpDbContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<ApiResponse<string>> Handle(DeleteTourSchedule request, CancellationToken cancellationToken)
        {
            var schedules = await _context.TourSchedules
                .Where(s => s.TourId == request.TourId
                            && s.OpenDate.HasValue
                            && s.OpenDate.Value.Date >= request.StartDay.ToDateTime(TimeOnly.MinValue)
                            && s.OpenDate.Value.Date <= request.EndDay.ToDateTime(TimeOnly.MaxValue)
                            && !s.IsDeleted)
                .Include(s => s.TourScheduleTickets)
                .Include(s => s.TourBookings)   // <-- include bookings
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
                return ApiResponse<string>.Failure("No tour schedules found …", 404);

            foreach (var schedule in schedules)
            {
                // refund tất cả booking Paid
                var paid = schedule.TourBookings
                                 .Where(tb => tb.Status == BookingStatus.Paid)
                                 .ToList();
                foreach (var booking in paid)
                {
                    booking.Cancel(request.Remark);
                    var payment = await _context.Payments
                        .FirstOrDefaultAsync(p => p.BookingId == booking.Id, cancellationToken);
                    if (payment != null)
                    {
                        await _publisher.Publish(
                            new PaymentRefunded(payment.NetCost, booking.UserId, booking.Code),
                            cancellationToken);
                        payment.Refund();
                    }
                }

                // đánh dấu xóa tickets và schedule
                schedule.TourScheduleTickets.ToList()
                    .ForEach(tkt => tkt.IsDeleted = true);
                schedule.IsDeleted = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Schedules deleted", "OK");
        }

    }
}
