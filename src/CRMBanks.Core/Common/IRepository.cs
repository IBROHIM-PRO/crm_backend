using System.Linq.Expressions;

namespace CRMBanks.Core.Common;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<List<T>> AddRangeAsync(List<T> entities);
    Task<T?> GetIdAsync(int id);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdWithIncludeAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null);
    IQueryable<T> GetQuery();
    IQueryable<T> GetQueryAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
    int SaveChanges();
    T Update(T entity);
    List<T> UpdateRange(List<T> entities);
    void Remove(T entity);
    void RemoveRange(List<T> entities);
    void IsDelete(T objModel);
}
