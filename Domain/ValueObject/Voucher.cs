namespace Domain.ValueObject;

public class Voucher
{
    public string Code { get; set; }
    public double Amount { get; set; }
    public double Percent { get; set; }
}