using Application.Common;
using Application.Contracts.Persistence;
using Application.Features.Wallet.Events; // PaymentRefunded
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Tour.Commands
{
    // Command để đóng tour, client gửi TourId và optional remark
    public record CloseTourCommand(Guid TourId, string? Remark) : IRequest<ApiResponse<string>>;

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
            // 1. Lấy Tour gồm các lịch và booking
            var tour = await _context.Tours
                .Include(t => t.TourSchedules)
                    .ThenInclude(ts => ts.TourBookings)
                .FirstOrDefaultAsync(t => t.Id == request.TourId, cancellationToken);

            if (tour == null)
                return ApiResponse<string>.Failure("Tour not found", 404);

            if (tour.IsDeleted)
            {
                tour.IsDeleted = false;
                _context.Tours.Update(tour);
                await _context.SaveChangesAsync(cancellationToken);
                return ApiResponse<string>.SuccessResult("Tour is opened ");
            }

            // 2. Tìm tất cả các booking đã Paid
            var paidBookings = await _context.TourBookings
                .Include(x => x.Tickets)
                .ThenInclude(x => x.TicketType)
                .Include(x => x.TourSchedule)
                .ThenInclude(x=> x.Tour)
                .AsSingleQuery()
                .Where(tb => tb.TourSchedule.TourId == tour.Id && (tb.Status == BookingStatus.Paid ||tb.Status==BookingStatus.AwaitingPayment))
                .ToListAsync(cancellationToken);

            foreach (var booking in paidBookings)
            {
                if (booking.Status == BookingStatus.Cancelled) {  booking.Cancel(request.Remark);

                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.BookingId == booking.Id, cancellationToken);

                if (payment != null)
                {
                    await _publisher.Publish(
                        new PaymentRefunded(payment.NetCost, booking.UserId, booking.Code),
                        cancellationToken
                    );
                    payment.Refund();
                    _context.Payments.Update(payment);
                }}

                booking.Cancel(request.Remark);
            }

            tour.IsDeleted = true;
            _context.Tours.Update(tour);

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<string>.SuccessResult("Tour closed successfully", "Closed");
        }
    }
}
