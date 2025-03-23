using Application.Contracts;
using Domain.Entities;
using Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OtpNet;

namespace Api.Filters;

public class OtpValidationFilter(
    UserManager<User> userManager,
    IUserContextService userService,
    ILogger<OtpValidationFilter> logger
) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if the method has the UseOtp attribute
        var useOtpAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<OtpAuthorizeAttribute>()
            .FirstOrDefault();

        if (useOtpAttribute != null)
        {
            var otpHeader = context.HttpContext.Request.Headers[useOtpAttribute.OtpHeaderName].FirstOrDefault();

            if (string.IsNullOrEmpty(otpHeader))
            {
                context.Result = new BadRequestObjectResult("OTP is required.");
                return;
            }

            var userId = userService.GetCurrentUserId()!;
            var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            // Validate the OTP
            if (user is not null && !ValidateOtp(user.OtpKey, otpHeader))
            {
                context.Result = new UnauthorizedObjectResult("Invalid OTP.");
                return;
            }
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

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class OtpAuthorizeAttribute(string otpHeaderName = "X-OTP") : Attribute
{
    public string OtpHeaderName { get; set; } = otpHeaderName;
}