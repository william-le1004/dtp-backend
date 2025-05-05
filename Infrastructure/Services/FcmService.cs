using Application.Contracts.Firebase;
using Domain.Constants;
using Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FcmService : IFcmService
{
    private readonly DtpDbContext _dbContext;
    private readonly ILogger<FcmService> _logger;
    private static bool _initialized;

    public FcmService(DtpDbContext dbContext, ILogger<FcmService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        
        if (!_initialized)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json")),
            });
            _initialized = true;
        }
    }

    public async Task SendNotificationAsync(string title, string body)
    {
        var tokens = await _dbContext.Users
            .Where(u => u.FcmToken != null)
            .Select(u => u.FcmToken)
            .ToListAsync();

        var messages = tokens.Select(token => new Message
        {
            Token = token,
            Notification = new Notification { Title = title, Body = body }
        });

        foreach (var message in messages)
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Successfully sent message: {Message}", message);
        }
    }
}