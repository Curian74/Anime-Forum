﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<List<T?>> GetAllWhereAsync(Expression<Func<T, bool>> predicate);    
        Task<T?> GetSingleWhereAsync(Expression<Func<T, bool>> predicate);
        Task<(IEnumerable<T> Items, int TotalCount)> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int size, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(object id);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task DeleteWhereAsync(Expression<Func<T, bool>> filter);
    }
}
