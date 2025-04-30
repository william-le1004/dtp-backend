using Application.Contracts.Persistence;
using Domain.Enum;
using MediatR;

namespace Application.Features.Voucher.Queries;

public record VoucherResponse
{
    public Guid Id { get; init; }
    public string Code { get; set; } = string.Empty;
    public decimal MaxDiscountAmount { get; set; }
    public double Percent { get; set; }
    public DateTime ExpiryDate { get; set; }
    
    public int Quantity { get; set; }
    
    public int AvailableVoucher { get; set; }
}

public record GetVouchers() : IRequest<IQueryable<VoucherResponse>>;

public class GetVouchersHandler(IDtpDbContext context) : IRequestHandler<GetVouchers, IQueryable<VoucherResponse>>
{
    public Task<IQueryable<VoucherResponse>> Handle(GetVouchers request, CancellationToken cancellationToken)
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
            });
        return Task.FromResult(vouchersWithAvailable);
    }
}