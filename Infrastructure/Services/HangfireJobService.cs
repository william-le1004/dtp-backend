using Application.Contracts.Job;
using Domain.Common;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class HangfireJobService(DtpDbContext dtpDbContext, ILogger<HangfireJobService> logger)
    : IHangfireJobService
{
    public async Task HardDeleteExpiredEntities()
    {
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
        var entityTypes = GetAuditEntityTypes();

        foreach (var entityType in entityTypes)
        {
            await HardDeleteEntities(entityType, oneMonthAgo);
        }
    }

    private async Task HardDeleteEntities(Type entityType, DateTime threshold)
    {
        try
        {
            var setMethod = typeof(DtpDbContext)
                .GetMethod(nameof(DtpDbContext.Set), Type.EmptyTypes)!
                .MakeGenericMethod(entityType);

            var dbSet = setMethod.Invoke(dtpDbContext, null) as IQueryable<AuditEntity>;

            if (dbSet == null) return;

            var entities = await dbSet.Where(e => !e.IsDeleted && e.CreatedAt <= threshold).ToListAsync();
            if (!entities.Any())
            {
                logger.LogDebug("No expired {EntityTypeName} records found for deletion.", entityType.Name);
                return;
            }

            dtpDbContext.RemoveRange(entities);
            dtpDbContext.ChangeTracker.Clear();
            await dtpDbContext.SaveChangesAsync();
            logger.LogInformation("{EntitiesCount} {EntityTypeName} records permanently deleted.",
                entities.Count, entityType.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hard delete cron job failed: {Message}", ex.Message);
            throw;
        }
    }

    private static List<Type> GetAuditEntityTypes()
    {
        // Get all types from Domain assembly that inherit from AuditEntity
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && a.FullName!.StartsWith("Domain"))
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AuditEntity)))
            .ToList();
    }
}