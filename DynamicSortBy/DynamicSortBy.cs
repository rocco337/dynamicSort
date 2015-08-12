using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSortBy
{
    public static class DynamicSortBy
    {
        public static IEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> list, string propertyName)
        {
            return list.AsQueryable().ApplyOrder(propertyName, "OrderBy");
        }

        public static IEnumerable<T> OrderByDescDynamic<T>(this IEnumerable<T> list, string propertyName)
        {
            return list.AsQueryable().ApplyOrder(propertyName, "OrderByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(this IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }
    }
}
