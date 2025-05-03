namespace Domain.DataModel;

public class SystemSetting
{
    public Guid Id { get; set; }
    public string SettingCode { get; set; }
    public string SettingKey { get; set; }
    public string SettingCharValue { get; set; }
    public string SettingIntegerValue { get; set; }
    public string SettingDoubleValue { get; set; }
    public string SettingDecimalValue { get; set; }
}