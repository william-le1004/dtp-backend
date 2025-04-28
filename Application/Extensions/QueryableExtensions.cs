using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> IsDeleted<T>(this IQueryable<T> source, bool isDeleted) where T : SoftDeleteEntity
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Where(x => x.IsDeleted == isDeleted);
    }
    
    public static IQueryable<T> ApplyNoTracking<T>(this IQueryable<T> query, bool noTracking) where T : class =>
        noTracking ? query.AsNoTracking() : query;
}