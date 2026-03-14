using CRMBanks.Core.Common;
using CRMBanks.Infrastructure.Data;
using CRMBanks.SharedKernel.Common.AbstractClasses;
using Microsoft.EntityFrameworkCore;
using EntityBaseWithDateCreated = CRMBanks.SharedKernel.Common.AbstractClasses.EntityBaseWithDateCreated;

namespace CRMBanks.Infrastructure;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DataContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(DataContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    public async Task<List<T>> AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await SaveChangesAsync();
        return entities;
    }

    public async Task<T?> GetIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
    }

    public async Task<T?> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetListAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {   
        return await _dbSet.ToListAsync(); 
    }
    
    public async Task<T?> GetByIdWithIncludeAsync(
        int id,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
            query = include(query);

        return await query.FirstOrDefaultAsync(e =>
            EF.Property<int>(e, "Id") == id
        );
    }

    public IQueryable<T> GetQuery()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<T> GetQueryAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate).AsQueryable();
    }

    public async Task<int> SaveChangesAsync()
    {
        UpdateTimestamps();
        return await _dbContext.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        UpdateTimestamps();
        return _dbContext.SaveChanges();
    }

    public T Update(T entity)
    {
        _dbSet.Update(entity);
        SaveChanges();
        return entity;
    }
    
    public List<T> UpdateRange(List<T> entities)
    {
        _dbSet.UpdateRange(entities);
        SaveChanges();
        return entities;
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
        SaveChanges();
    }

    public void RemoveRange(List<T> entities)
    {
        _dbSet.RemoveRange(entities);
        SaveChanges();
    }
    
    public void IsDelete(T objModel)
    {
        if (objModel is EntityProduction e)
        {
            e.DateDeletedAt = DateTime.UtcNow;
            e.IsDeleted = true;
        }

        Update(objModel);
    }
    
    private void UpdateTimestamps()
    {
        var entries = _dbContext.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry is { Entity: EntityBaseWithDateCreated baseEntity, State: EntityState.Added })
            {
                baseEntity.DateCreated = DateTime.UtcNow;
            }
            
            if (entry.Entity is EntityProduction productionEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        productionEntity.DateCreated = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        productionEntity.DateUpdated = DateTime.UtcNow;
                        break; 
                }
            }
        }
    }
}
