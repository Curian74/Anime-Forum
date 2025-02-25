using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<(IEnumerable<T> Items, int TotalCount)> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int size, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(object id);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task DeleteWhereAsync(Expression<Func<T, bool>> filter);
    }
}
