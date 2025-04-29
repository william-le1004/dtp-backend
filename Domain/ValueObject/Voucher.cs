namespace Domain.ValueObject;

public class Voucher
{
    public string Code { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public double Percent { get; set; }
    public DateTime ExpiryDate { get; set; }
    
    public int Quantity { get; set; }
    
    public int AvailableVoucher { get; }

    public bool IsValid()
    {
        // Check if the voucher is expired
        return DateTime.Now <= ExpiryDate;
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
}