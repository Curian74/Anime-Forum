using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>>? BuildFilterExpression<T>(string? filterBy, string? searchTerm)
        {
            if (string.IsNullOrEmpty(filterBy) || string.IsNullOrEmpty(searchTerm))
                return null;

            var property = typeof(T).GetProperty(filterBy);
            if (property == null) return null;

            var parameter = Expression.Parameter(typeof(T), "p");
            var propertyAccess = Expression.Property(parameter, property);
            var convertedSearchTerm = Convert.ChangeType(searchTerm, property.PropertyType);
            var searchExpression = Expression.Constant(convertedSearchTerm);

            Expression comparison = property.PropertyType == typeof(string)
                ? Expression.Call(propertyAccess, nameof(string.Contains), Type.EmptyTypes, searchExpression)
                : Expression.Equal(propertyAccess, searchExpression);

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>>? BuildOrderExpression<T>(string? orderBy, bool descending)
        {
            if (string.IsNullOrEmpty(orderBy))
                return null;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            return descending
                ? (Func<IQueryable<T>, IOrderedQueryable<T>>)(q => q.OrderByDescending(lambda))
                : q => q.OrderBy(lambda);
        }
    }
}
