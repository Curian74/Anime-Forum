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

            // Xu ly nested Props
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

            // Check Guid type truoc
            object convertedSearchTerm;
            try
            {
                if (propertyType == typeof(Guid))
                {
                    convertedSearchTerm = Guid.Parse(searchTerm);
                }
                else
                {
                    convertedSearchTerm = Convert.ChangeType(searchTerm, propertyType);
                }
            }
            catch
            {
                return null; // Null neu k convert duoc
            }

            var searchExpression = Expression.Constant(convertedSearchTerm);

            Expression comparison = propertyType == typeof(string)
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
                ? (Func<IQueryable<T>, IOrderedQueryable<T>>)(q => q.OrderByDescending(lambda))
                : q => q.OrderBy(lambda);
        }
    }
}
