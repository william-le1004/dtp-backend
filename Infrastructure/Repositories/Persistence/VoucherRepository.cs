using Application.Contracts.Persistence;
using Domain.Enum;
using Domain.ValueObject;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Persistence;

public class VoucherRepository(DtpDbContext context) : IVoucherRepository
{
    public async Task<Voucher?> GetVoucherByCodeAsync(string? code)
    {
        var result = await context.Voucher
            .Select(v => new
            {
                Voucher = v,
                Used = context.TourBookings.Count(b => b.VoucherCode == v.Code && b.Status == BookingStatus.Paid)
            }).FirstOrDefaultAsync(x=> x.Voucher.Code == code);

        if (result is not null)
        {
            result.Voucher.CalAvailableVoucher(result.Used);
            return result.Voucher;
        }
        return null;
    }
}