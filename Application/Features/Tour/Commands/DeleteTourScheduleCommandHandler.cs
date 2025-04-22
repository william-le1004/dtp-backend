using Application.Common;
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

        public DeleteTourScheduleHandler(IDtpDbContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<ApiResponse<string>> Handle(DeleteTourSchedule request, CancellationToken cancellationToken)
        {
            // Lấy tất cả các TourSchedule của Tour có TourId = request.TourId và nằm trong khoảng thời gian được chỉ định
            var schedules = await _context.TourSchedules
                .Where(s => s.TourId == request.TourId &&
                            s.OpenDate.HasValue && s.OpenDate.Value.Date >= request.StartDay.ToDateTime(TimeOnly.MinValue) &&
                            s.CloseDate.HasValue && s.CloseDate.Value.Date <= request.EndDay.ToDateTime(TimeOnly.MinValue))
                .Include(s => s.TourScheduleTickets)
                .Include(s => s.Tour)
                .ThenInclude(t => t.Company)
                .ToListAsync(cancellationToken);

            if (!schedules.Any())
            {
                return ApiResponse<string>.Failure("No tour schedules found in the given date range for the specified Tour", 404);
            }

            foreach (var schedule in schedules)
            {
                // Kiểm tra các booking có trạng thái Paid cho schedule hiện tại
                var paidBookings = await _context.TourBookings
                    .Where(tb => tb.TourScheduleId == schedule.Id && tb.Status == BookingStatus.Paid)
                    .ToListAsync(cancellationToken);

                foreach (var booking in paidBookings)
                {
                    // Lấy Payment tương ứng với booking
                    var payment = await _context.Payments
                        .Include(p => p.Booking)
                        .ThenInclude(b => b.TourSchedule)
                        .FirstOrDefaultAsync(p => p.BookingId == booking.Id, cancellationToken);
                    booking.Cancel(request.Remark);
                    if (payment != null)
                    {
                        // Trong trường hợp này, hoàn tiền 100%
                        decimal refundAmount = payment.NetCost;

                        // Publish TourCancelled event
                        await _publisher.Publish(new TourCancelled(
                            CompanyName: schedule.Tour.Company.Name,
                            TourTitle: schedule.Tour.Title,
                            BookingCode: booking.Code,
                            CustomerName: booking.Name,
                            StartDate: schedule.OpenDate.Value,
                            Remark: request.Remark ?? "Tour cancelled by admin",
                            PaidAmount: payment.NetCost,
                            RefundAmount: refundAmount
                        ), cancellationToken);

                        // Publish PaymentRefunded event
                        await _publisher.Publish(new PaymentRefunded(refundAmount, booking.UserId, booking.Code), cancellationToken);

                        // Gọi refund trên Payment (cập nhật trạng thái, ...)
                        payment.Refund();
                        _context.Payments.Update(payment);
                    }
                }

                // Sau khi hoàn tiền (nếu cần), đánh dấu các vé lịch trình và schedule là deleted
                foreach (var ticket in schedule.TourScheduleTickets.ToList())
                {
                    ticket.IsDeleted = true;
                }
                schedule.IsDeleted = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Tour schedules and associated tickets deleted successfully", "Deletion successful");
        }
    }
}
