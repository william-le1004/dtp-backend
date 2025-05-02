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
    private readonly UserManager<User> _userManager;
    private readonly ILogger<FcmService> _logger;
    private static bool _initialized;

    public FcmService(DtpDbContext dbContext, UserManager<User> userManager, ILogger<FcmService> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
        
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
        var credentialsPath = Path.Combine(projectRoot, "firebase-adminsdk.json");
        if (!_initialized)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(credentialsPath),
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
            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _logger.LogInformation("Successfully sent message: {Message}", message);
        }
    }

    public async Task<string> SendNotificationAsync(string title, string body, string token)
    {
        var message = new Message
        {
            Token = token,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        return response;
    }
}