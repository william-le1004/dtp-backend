using Application.Contracts.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.DataModel;
using Infrastructure.Contexts;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController(DtpDbContext context, ISystemSettingService service) : ControllerBase
{
    // GET: api/System
    [HttpGet]
    public ActionResult<IEnumerable<SystemSettingResponse>> GetSystemSetting()
    {
        var result =  context.SystemSetting.AsEnumerable().Select(x => new SystemSettingResponse()
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
    public async Task<IActionResult> PutSystemSetting(SystemSettingRequest requests)
    {
        var oldSetting = await context.SystemSetting.FirstOrDefaultAsync(x => x.Id == requests.Id);
        if (oldSetting is not null)
        {
            oldSetting.SettingValue = requests.SettingValue;
            context.SystemSetting.Update(oldSetting);
            await context.SaveChangesAsync();
            await service.UpdateSettingAsync(oldSetting);
            return NoContent();
        }
        return BadRequest();
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

public record SystemSettingResponse
{
    public Guid Id { get; set; }
    public string SettingCode { get; set; }
    public string SettingKey { get; set; }
    public long SettingValue { get; set; }
}

public record SystemSettingRequest
{
    public Guid Id { get; set; }
    public long SettingValue { get; set; }
}