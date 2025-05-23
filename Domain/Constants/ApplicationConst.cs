namespace Domain.Constants;

public static class ApplicationConst
{
    public const string RefreshTokenPrefix = "RF";
    public const string BlacklistPrefix = "BL";
    public const string HangfirePrefix = "HF";


    public const string DefaultPassword = "1A@a";
    public const string GmailDomain = "@gmail.com";

    public const string AuthenticatedUser = "AuthenticatedUser";
    public const string AdminPermission = "AdminPermission";
    public const string ManagementPermission = "ManagementPermission";
    public const string HighLevelPermission = "HighLevelPermission";

    public const string XOtp = "X-OTP";
    public const string AppName = "Binh Dinh Tour";
    
    public const string CancelOrderQueue = "cancel-order";
    public const string CompleteOrderQueue = "complete-order";
}