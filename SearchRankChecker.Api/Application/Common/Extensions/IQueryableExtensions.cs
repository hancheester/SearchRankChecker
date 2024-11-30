using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System;
using SearchRankChecker.Api.Application.Common.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;
using SearchRankChecker.Api.Application.Common.Result;
using System.Linq.Dynamic.Core;

namespace SearchRankChecker.Api.Application.Common.Extensions;

public static class IQueryableExtensions
{
    public static async Task<IResult<IPagedList<T>>> ToPaginatedListAsync<T>(this IQueryable source, int pageIndex, int pageSize, Func<List<dynamic>, List<T>> mapFrom) where T : class
    {
        var totalCount = source.Count();

        var dynamicResult = await source.Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToDynamicListAsync();

        var result = mapFrom(dynamicResult);

        return await BuildPagedListResultAsync(pageIndex, pageSize, totalCount, result);
    }

    public static IQueryable<TSource> Contains<TSource>(this IQueryable<TSource> source, string propertyName, string propertyValue)
    {
        if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue))
        {
            return source;
        }

        // LAMBDA: x => x.[PropertyName].Contains([PropertyValue])
        var parameter = Expression.Parameter(typeof(TSource), "x");
        var property = Expression.Property(parameter, propertyName);

        MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var keyword = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(property, containsMethod, keyword);

        var lambda = Expression.Lambda<Func<TSource, bool>>(containsMethodExp, parameter);

        // REFLECTION: source.Where(x => x.[PropertyName].Contains([PropertyValue]))
        var whereMethod = typeof(Queryable).GetMethods().First(x => x.Name == "Where" && x.GetParameters().Length == 2);
        var whereGeneric = whereMethod.MakeGenericMethod(typeof(TSource));
        var result = whereGeneric.Invoke(null, new object[] { source, lambda });

        return (IQueryable<TSource>)result;
    }

    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        // LAMBDA: x => x.[PropertyName]
        var parameter = Expression.Parameter(typeof(TSource), "x");
        var propertyNames = propertyName.Split('.');
        Expression property;

        if (propertyNames.Length == 2)
        {
            var property1 = typeof(TSource).GetProperty(UppercaseFirst(propertyNames[0]));
            var property2 = property1.PropertyType.GetProperty(UppercaseFirst(propertyNames[1]));
            var inner = Expression.Property(parameter, property1);
            property = Expression.Property(inner, property2);
        }
        else
        {
            property = Expression.Property(parameter, propertyName);
        }

        var lambda = Expression.Lambda(property, parameter);

        // REFLECTION: source.OrderBy(x => x.Property)
        var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
        var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TSource), property.Type);
        var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

        return (IOrderedQueryable<TSource>)result;
    }

    public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        // LAMBDA: x => x.[PropertyName]
        var parameter = Expression.Parameter(typeof(TSource), "x");
        var propertyNames = propertyName.Split('.');
        Expression property;

        if (propertyNames.Length == 2)
        {
            var property1 = typeof(TSource).GetProperty(UppercaseFirst(propertyNames[0]));
            var property2 = property1.PropertyType.GetProperty(UppercaseFirst(propertyNames[1]));
            var inner = Expression.Property(parameter, property1);
            property = Expression.Property(inner, property2);
        }
        else
        {
            property = Expression.Property(parameter, propertyName);
        }

        var lambda = Expression.Lambda(property, parameter);

        // REFLECTION: source.OrderByDescending(x => x.Property)
        var orderByDescendingMethod = typeof(Queryable).GetMethods().First(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
        var orderByDescendingGeneric = orderByDescendingMethod.MakeGenericMethod(typeof(TSource), property.Type);
        var result = orderByDescendingGeneric.Invoke(null, new object[] { source, lambda });

        return (IOrderedQueryable<TSource>)result;
    }

    public static async Task<IResult<IPagedList<T>>> BuildPagedListResultAsync<T>(int pageIndex, int pageSize, int totalCount, IList<T> result) where T : class
    {
        var pagedList = new PagedList<T>
        {
            Items = result,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalCount = totalCount,
        };

        return await Result<PagedList<T>>.SuccessAsync(pagedList);
    }

    private static string UppercaseFirst(string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
