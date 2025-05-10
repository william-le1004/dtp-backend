using Application.Common;
using Application.Contracts.EventBus;
using Application.Contracts.Persistence;
using Application.Features.Wallet.Events;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var today = DateTime.Today;

            var schedules = await _context.TourSchedules
                .Where(s => s.TourId == request.TourId
                            && s.OpenDate.HasValue
                            && s.OpenDate.Value.Date >= request.StartDay.ToDateTime(TimeOnly.MinValue)
                            && s.OpenDate.Value.Date <= request.EndDay.ToDateTime(TimeOnly.MaxValue)
                            && !s.IsDeleted)
                .Include(s => s.TourScheduleTickets)
                .Include(s => s.TourBookings)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
                return ApiResponse<string>.Failure(
                    "No tour schedules found in the given date range for the specified Tour", 404);

            foreach (var schedule in schedules)
            {
                var departureDate = schedule.OpenDate!.Value.Date;

                if (departureDate > today)
                {
                    // -- CHỈ với những lịch còn tương lai --
                    // 1) Hủy và hoàn tiền các booking Paid
                    var paidBookings = schedule.TourBookings
                        .Where(tb => tb.Status == BookingStatus.Paid&&tb.Status==BookingStatus.AwaitingPayment)
                        .ToList();

                    foreach (var booking in paidBookings)
                    {
                        booking.Cancel(request.Remark);

                        var payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.BookingId == booking.Id, cancellationToken);

                        if (payment != null)
                        {
                            // Phát event hoàn tiền
                            await _publisher.Publish(
                                new PaymentRefunded(payment.NetCost, booking.UserId, booking.Code),
                                cancellationToken);

                            payment.Refund();
                        }
                    }

                    // 2) Đánh dấu các vé lịch trình là deleted
                    foreach (var tkt in schedule.TourScheduleTickets)
                        tkt.IsDeleted = true;
                }

                // -- Với cả lịch tương lai và lịch đúng hôm nay --
                // 3) Đánh dấu chính lịch là deleted
                schedule.IsDeleted = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Tour schedules processed successfully", "OK");
        }
    }
}