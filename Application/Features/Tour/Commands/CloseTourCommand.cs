using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Wallet.Events; // Giả sử PaymentRefunded được định nghĩa ở đây
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // Command để đóng tour; client chỉ cần truyền TourId
    public record CloseTourCommand(Guid TourId, string? remark) : IRequest<ApiResponse<string>>;

    public class CloseTourHandler : IRequestHandler<CloseTourCommand, ApiResponse<string>>
    {
        private readonly IDtpDbContext _context;
        private readonly IPublisher _publisher;

        public CloseTourHandler(IDtpDbContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<ApiResponse<string>> Handle(CloseTourCommand request, CancellationToken cancellationToken)
        {
            // Lấy Tour theo TourId, bao gồm các TourSchedules và trong đó các TourBookings
            var tour = await _context.Tours
                .Include(t => t.TourSchedules)
                    .ThenInclude(ts => ts.TourBookings)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

            if (tour == null)
            {
                return ApiResponse<string>.Failure("Tour not found", 404);
            }

            // Nếu tour đã đóng (IsDeleted == true) thì không cần cập nhật
            if (tour.IsDeleted)
            {
                return ApiResponse<string>.SuccessResult("Tour is already closed", "No update needed");
            }

            // Xử lý hoàn tiền cho các booking Paid của tour:
            // Lấy tất cả các booking (qua các lịch trình) có trạng thái Paid
            var paidBookings = await _context.TourBookings
                .Include(tb => tb.Tickets)
                .Include(tb => tb.TourSchedule)
                .Where(tb => tb.TourSchedule.TourId == tour.Id && tb.Status == BookingStatus.Paid)
                .ToListAsync(cancellationToken);

            foreach (var booking in paidBookings)
            {
                // Giả sử phương thức IsFreeCancellationPeriod() và IsBeforeStartDate(int days) được định nghĩa ở Booking và TourSchedule
                var payment = await _context.Payments
                    .Include(p => p.Booking)
                    .ThenInclude(b => b.TourSchedule)
                    .FirstOrDefaultAsync(p => p.BookingId == booking.Id, cancellationToken);
                booking.Cancel(request.remark);
                if (payment is not null)
                {
                 
                        // Phát hành sự kiện hoàn tiền cho người dùng, thông tin UserId và Booking Code được lấy từ booking
                        await _publisher.Publish(new PaymentRefunded(payment.NetCost, booking.UserId, booking.Code), cancellationToken);
                        // Gọi phương thức Refund() trên Payment để cập nhật trạng thái
                        payment.Refund();
                        _context.Payments.Update(payment);
                                  
            }

            // Sau khi xử lý hoàn tiền cho các booking đã thanh toán,
            // đánh dấu tour là đóng bằng cách set IsDeleted = true
            tour.IsDeleted = true;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<string>.SuccessResult("Tour closed successfully", "Closed");
        }
    }
}
