using Application.Common.Interfaces.Repository.Base;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository;

public class Repository<T> : RepositoryRead<T>, IRepository<T>
    where T : BaseEntity
{
    protected readonly DbSet<T> dbSet;
    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
        dbSet = dbContext.Set<T>();
        _dbContext = dbContext;
    }

    public async Task LoadReferenceAsync<TProperty>(T entity, Expression<Func<T, TProperty>> navigationProperty, CancellationToken cancellationToken)
         where TProperty : class
    {
        await _dbContext.Entry(entity).Reference(navigationProperty!).LoadAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellation = default)
    {
        var result = await dbSet.AddAsync(entity, cancellation);
        return result.Entity;
    }

    public async Task<IReadOnlyCollection<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        var entityList = entities.ToList();
        if (entityList.Count == 0) return new List<T>();

        await dbSet.AddRangeAsync(entityList, cancellation);
        return entityList;
    }

    public T Delete(T entity)
    {
        return dbSet.Remove(entity).Entity;
    }

    public async Task<T?> DeleteAsync(int id, CancellationToken cancellation = default)
    {
        var entityToDelete = await GetSafeAsync(id, cancellation);
        if (entityToDelete == null) return null;

        return Delete(entityToDelete);
    }

    public IReadOnlyCollection<T> DeleteRange(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        if (entityList.Count == 0) return new List<T>();

        dbSet.RemoveRange(entityList);
        return entityList;
    }

    public T Update(T entity)
    {
        return dbSet.Update(entity).Entity;
    }

    public IReadOnlyCollection<T> Update(IEnumerable<T> entities)
    {
        var entityList = entities.ToList();
        if (entityList.Count == 0) return new List<T>();

        dbSet.UpdateRange(entityList);

        return entityList;
    }
}
