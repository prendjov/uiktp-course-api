using Domain.Entities.Base;

namespace Application.Common.Interfaces.Repository.Base;

public interface IRepositoryRead<T>
    where T : BaseEntity
{
    Task<T?> GetAsync(int id, CancellationToken cancellation = default);
    Task<T> GetSafeAsync(int id, CancellationToken cancellation = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellation = default);
    Task<IReadOnlyList<T>> GetManyAsync(IEnumerable<int> ids, CancellationToken cancellation = default);
}
