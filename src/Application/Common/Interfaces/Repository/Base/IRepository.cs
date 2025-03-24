using Domain.Entities.Base;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repository.Base;

public interface IRepository<T> : IRepositoryRead<T>
    where T : BaseEntity
{
    Task<T> AddAsync(T entity, CancellationToken cancellation = default);
    Task<IReadOnlyCollection<T>> AddRangeAsync(IEnumerable<T> entity, CancellationToken cancellation = default);
    T Update(T entity);
    IReadOnlyCollection<T> Update(IEnumerable<T> entity);
    T Delete(T entity);
    Task<T?> DeleteAsync(int key, CancellationToken cancellation = default);
    IReadOnlyCollection<T> DeleteRange(IEnumerable<T> entities);
    Task LoadReferenceAsync<TProperty>(T entity, Expression<Func<T, TProperty>> navigationProperty, CancellationToken cancellationToken) 
        where TProperty : class;
}