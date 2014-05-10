using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace pCloud.Data
{
    internal static class QueryableSorting
    {
        private static readonly MethodInfo orderBy, orderByDescending, thenBy, thenByDescending;
        static QueryableSorting()
        {
            orderBy = GetMethod(() => Queryable.OrderBy<int, int>(null, null));
            orderByDescending = GetMethod(() => Queryable.OrderByDescending<int, int>(null, null));
            thenBy = GetMethod(() => Queryable.ThenBy<int, int>(null, null));
            thenByDescending = GetMethod(() => Queryable.ThenByDescending<int, int>(null, null));
        }

        internal static IOrderedQueryable<T> Sort<T>(this IQueryable<T> queryable, IEnumerable<SortDescriptorBase> sortDescriptors)
        {
            if (!sortDescriptors.Any())
            {
                return queryable.OrderBy(item => item);
            }

            var itemParameter = Expression.Parameter(typeof(T));
            var firstSortDescriptor = sortDescriptors.First();
            Expression expression = Expression.Constant(queryable);
            
            var keySelector = firstSortDescriptor.CreateSortKeyLambda(itemParameter);
            if (firstSortDescriptor.Direction == ListSortDirection.Ascending)
            {
                expression = Expression.Call(orderBy.MakeGenericMethod(typeof(T), keySelector.Body.Type), expression, keySelector);
            }
            else
            {
                expression = Expression.Call(orderByDescending.MakeGenericMethod(typeof(T), keySelector.Body.Type), expression, keySelector);
            }

            foreach (var sortDescriptor in sortDescriptors.Skip(1))
            {
                keySelector = sortDescriptor.CreateSortKeyLambda(itemParameter);
                if (sortDescriptor.Direction == ListSortDirection.Ascending)
                {
                    expression = Expression.Call(thenBy.MakeGenericMethod(typeof(T), keySelector.Body.Type), expression, keySelector);
                }
                else
                {
                    expression = Expression.Call(thenByDescending.MakeGenericMethod(typeof(T), keySelector.Body.Type), expression, keySelector);
                }
            }

            return Expression.Lambda<Func<IOrderedQueryable<T>>>(expression).Compile().Invoke();
        }

        private static MethodInfo GetMethod(Expression<Action> expression)
        {
            return ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition();
        }
    }
}
