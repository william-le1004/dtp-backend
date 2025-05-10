using Domain.ValueObject;

namespace Application.Contracts.Persistence;

public interface IVoucherRepository
{
    Task<Voucher?> GetVoucherByCodeAsync(string? code);
}