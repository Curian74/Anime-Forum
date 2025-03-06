using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>>? BuildFilterExpression<T>(string? filterBy, string? searchTerm)
        {
            if (string.IsNullOrEmpty(filterBy) || string.IsNullOrEmpty(searchTerm))
                return null;

            var parameter = Expression.Parameter(typeof(T), "p");

            // Handle nested properties
            Expression? propertyAccess = parameter;
            foreach (var propertyName in filterBy.Split('.'))
            {
                propertyAccess = Expression.PropertyOrField(propertyAccess, propertyName);
                if (propertyAccess == null)
                {
                    return null;
                }
            }

            var propertyType = propertyAccess.Type;

            // Handle nullable types by getting the underlying type
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            // Try to convert the search term
            object convertedSearchTerm;
            try
            {
                if (underlyingType == typeof(Guid))
                {
                    convertedSearchTerm = Guid.Parse(searchTerm);
                }
                else
                {
                    convertedSearchTerm = Convert.ChangeType(searchTerm, underlyingType);
                }
            }
            catch
            {
                return null; // If conversion fails
            }

            var searchExpression = Expression.Constant(convertedSearchTerm, propertyType);

            // Build comparison expression
            Expression comparison = underlyingType == typeof(string)
                ? Expression.Call(propertyAccess, nameof(string.Contains), Type.EmptyTypes, searchExpression)
                : Expression.Equal(propertyAccess, searchExpression);

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }



        public static Func<IQueryable<T>, IOrderedQueryable<T>>? BuildOrderExpression<T>(string? orderBy, bool descending)
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            return descending
                ? q => q.OrderByDescending(lambda)
                : q => q.OrderBy(lambda);
        }
    }
}
