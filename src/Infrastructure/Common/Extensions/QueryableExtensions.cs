using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Infrastructure.Persistence.Converters;
using MyReliableSite.Shared.DTOs.Filters;
using System.Linq.Dynamic.Core;

namespace MyReliableSite.Infrastructure.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec)
        where T : BaseEntity
    {
        var queryableResultWithIncludes = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));
        var secondaryResult = spec.IncludeStrings
            .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));
        if (spec.Criteria == null)
            return secondaryResult;
        else
            return secondaryResult.Where(spec.Criteria);
    }

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filters<T> filters)
    {
        if (filters?.IsValid() == true)
            query = filters.Get().Aggregate(query, (current, filter) => current.Where(filter.Expression));
        return query;
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string[] orderBy, OrderTypeEnum? orderTypeEnum)
        where T : BaseEntity
    {
        string ordering = new OrderByConverter().ConvertBack(orderBy);
        return !string.IsNullOrWhiteSpace(ordering) ? query.OrderBy(ordering + (orderTypeEnum == OrderTypeEnum.OrderByDescending ? " desc" : string.Empty)) : query.OrderBy(a => a.Id);

    }
}