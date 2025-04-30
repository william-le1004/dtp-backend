using System.ComponentModel.DataAnnotations;
using Application.Contracts.Persistence;
using MediatR;
using VoucherEntity = Domain.ValueObject.Voucher;

namespace Application.Features.Voucher.Commands;
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date > DateTime.Now;
        }
        return false;
    }
}
public record CreateVoucherCommand : IRequest<Guid>
{
    
    [Range(0.01, 1000, ErrorMessage = "Max discount amount must be between 0.01 and 1000.")]
    public decimal MaxDiscountAmount { get; set; }

    [Range(0.01, 100, ErrorMessage = "Discount percentage must be between 0.01 and 100.")]
    public double Percent { get; set; }

    [Required(ErrorMessage = "Expiry date is required.")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage = "Expiry date must be in the future.")]
    public required DateTime ExpiryDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
    
    [MaxLength(500, ErrorMessage = "Description must be less than 500 characters.")]
    public string? Description { get; set; }
}

public class CreateVoucherCommandHandler(IDtpDbContext context) : IRequestHandler<CreateVoucherCommand, Guid>
{
    public Task<Guid> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = new VoucherEntity(
            request.MaxDiscountAmount,
            request.Percent, 
            request.ExpiryDate, 
            request.Quantity,
            request.Description);
        
        context.Voucher.Add(voucher);
        context.SaveChangesAsync(cancellationToken);
        return Task.FromResult(voucher.Id);
    }
}