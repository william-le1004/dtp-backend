using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Voucher.Commands;

public record UpdateVoucherCommand : IRequest
{
    [IgnoreDataMember]
    public Guid Id { get; set; }
    
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

public class UpdateVoucherCommandHandler(IDtpDbContext context) : IRequestHandler<UpdateVoucherCommand>
{
    public async Task Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        var persistenceVoucher = await context.Voucher
            .FirstOrDefaultAsync(x=> x.Id == request.Id, cancellationToken);
        if (persistenceVoucher is not null)
        {
            persistenceVoucher.Description = request.Description ?? String.Empty;
            persistenceVoucher.ExpiryDate = request.ExpiryDate;
            persistenceVoucher.Quantity = request.Quantity;
            persistenceVoucher.MaxDiscountAmount = request.MaxDiscountAmount;
            persistenceVoucher.Percent = request.Percent;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}