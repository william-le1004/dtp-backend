using Domain.Common;

namespace Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> IsDeleted<T>(this IQueryable<T> source, bool isDeleted) where T : AuditEntity
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Where(x => x.IsDeleted == isDeleted);
    }
}