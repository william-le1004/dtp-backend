using Application.Contracts;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OtpNet;

namespace Api.Filters;

public class OtpAuthorizeFilter(
    UserManager<User> userManager,
    IUserContextService userService,
    ILogger<OtpAuthorizeFilter> logger
) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var otpHeader = context.HttpContext.Request.Headers[ApplicationConst.XOtp].FirstOrDefault();

        if (string.IsNullOrEmpty(otpHeader))
        {
            context.Result = new BadRequestObjectResult("OTP is required.");
            return;
        }

        var userId = userService.GetCurrentUserId()!;
        var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult()!;

        if (user.OtpKey is null)
        {
            context.Result = new BadRequestObjectResult("OTP is not configured.");
            return;
        }
        
        // Validate the OTP
        if (!ValidateOtp(user.OtpKey, otpHeader))
        {
            context.Result = new UnauthorizedObjectResult("Invalid OTP.");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("Otp validation executed");
    }

    private bool ValidateOtp(string secretKey, string otpCode)
    {
        var otp = new Totp(Base32Encoding.ToBytes(secretKey));
        return otp.VerifyTotp(otpCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}