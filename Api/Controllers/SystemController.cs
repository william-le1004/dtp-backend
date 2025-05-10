using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.DataModel;
using Infrastructure.Contexts;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController(DtpDbContext context) : ControllerBase
{
    // GET: api/System
    [HttpGet]
    public ActionResult<IEnumerable<SystemSettingDto>> GetSystemSetting()
    {
        var result =  context.SystemSetting.AsEnumerable().Select(x => new SystemSettingDto()
        {
            Id = x.Id,
            SettingValue = x.SettingValue,
            SettingCode = MapSettingCodeToVn(x.SettingCode),
            SettingKey = MapSettingKeyToVn(x.SettingKey)
        }).ToList();
        return result;
    }

    // PUT: api/System/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut]
    public async Task<IActionResult> PutSystemSetting(List<SystemSettingDto> dtos)
    {
        var settingIds= dtos.Select(x => x.Id).ToList();
        var oldSetting = context.SystemSetting.Where(x => settingIds.Contains(x.Id)).ToList();
        
        foreach (var setting in oldSetting)
        {
            setting.SettingValue = dtos.FirstOrDefault(x => x.Id == setting.Id)?.SettingValue ?? setting.SettingValue;
        }
        await context.SaveChangesAsync();
        return NoContent();
    }

    private string MapSettingCodeToVn(SettingCode settingCode)
    {
        return settingCode switch
        {
            SettingCode.Refund => "Hoàn tiền",
            _ => string.Empty
        };
    }

    private string MapSettingKeyToVn(SettingKey settingKey)
    {
        return settingKey switch
        {
            SettingKey.FreeCancellationPeriod => "Thời gian(date) miễn phí hoàn tiền",
            SettingKey.CancelFee => "Phần trăm hoàn tiền chịu phí",
            SettingKey.NonRefundablePeriod => "Thời gian(date) không hoàn tiền(truớc ngày bắt đầu tour)",
            _ => string.Empty
        };
    }
}

public record SystemSettingDto
{
    public Guid Id { get; set; }
    public string SettingCode { get; set; }
    public string SettingKey { get; set; }
    public long SettingValue { get; set; }
}