namespace Domain.DataModel;

public class SystemSetting
{
    public Guid Id { get; set; }
    public SettingCode SettingCode { get; set; }
    public SettingKey SettingKey { get; set; }
    public long SettingValue { get; set; }
}

public enum SettingCode
{
    Refund,
}

public enum SettingKey
{
    FreeCancellationPeriod,
    CancelFee,
    NonRefundablePeriod,
}