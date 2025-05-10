using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;

namespace Application.Features.Voucher.Queries;

public record GetOwnVoucher(): IRequest<IQueryable<VoucherResponse>>;

public class GetOwnVoucherHandler(IDtpDbContext context) : IRequestHandler<GetOwnVoucher, IQueryable<VoucherResponse>>
{
    public Task<IQueryable<VoucherResponse>> Handle(GetOwnVoucher request, CancellationToken cancellationToken)
    {
        var vouchersWithAvailable = context.Voucher
            .Select(v => new
            {
                Voucher = v,
                Used = context.TourBookings.Count(b => b.VoucherCode == v.Code && b.Status == BookingStatus.Paid)
            })
            .Select(x => new VoucherResponse()
            {
                Id = x.Voucher.Id,
                Code = x.Voucher.Code,
                Quantity = x.Voucher.Quantity,
                AvailableVoucher = x.Voucher.Quantity - x.Used,
                MaxDiscountAmount = x.Voucher.MaxDiscountAmount,
                Percent = x.Voucher.Percent,
                ExpiryDate = x.Voucher.ExpiryDate,
                Description = x.Voucher.Description,
                IsDeleted = x.Voucher.IsDeleted,
                CreatedAt = x.Voucher.CreatedAt,
                
            });
        return Task.FromResult(vouchersWithAvailable);
    }
}