using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Monte.Models;

namespace Monte.Extensions;

public static class QueryExtensions
{
    public static IOrderedQueryable<TEntity> OrderBy<TEntity, TProp>(this IQueryable<TEntity> query,
        Expression<Func<TEntity, TProp>> expression,
        bool descending)
        => descending
            ? query.OrderByDescending(expression)
            : query.OrderBy(expression);

    public static async Task<PagedResponse<T>> PaginateAsync<T>(this IQueryable<T> query,
        Paging paging,
        CancellationToken cancellationToken = default)
    {
        var count = await query.CountAsync(cancellationToken);

        var skipCount = paging.Page * paging.PageSize;
        var items = await query.Skip(skipCount)
            .Take(paging.PageSize)
            .ToArrayAsync(cancellationToken);

        return new(items, paging, count);
    }
}
