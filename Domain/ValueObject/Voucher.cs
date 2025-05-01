using Domain.Extensions;

namespace Domain.ValueObject;

public class Voucher : AuditEntity
{
    public string Code { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public double Percent { get; set; }
    public DateTime ExpiryDate { get; set; }

    public int Quantity { get; set; }
    public string Description { get; set; }

    public int AvailableVoucher { get; private set; }


    public Voucher()
    {
    }

    public Voucher(decimal maxDiscountAmount,
        double percent, DateTime expiryDate,
        int quantity, string description)
    {
        Code = StringExtensions.GenerateCode();
        MaxDiscountAmount = maxDiscountAmount;
        Percent = percent;
        ExpiryDate = expiryDate;
        Quantity = quantity;
        Description = description;
        AvailableVoucher = Quantity;
    }

    public bool IsValid()
    {
        // Check if the voucher is expired
        return DateTime.Now <= ExpiryDate && AvailableVoucher > 0;
    }

    public decimal ApplyVoucherDiscount(decimal orderTotal)
    {
        if (Percent > 0)
        {
            var discountAmount = orderTotal * (decimal)Percent;

            return discountAmount > MaxDiscountAmount ? MaxDiscountAmount : discountAmount;
        }

        return MaxDiscountAmount;
    }

    public void CalAvailableVoucher(int used)
    {
        AvailableVoucher = Quantity - used;
    }
}