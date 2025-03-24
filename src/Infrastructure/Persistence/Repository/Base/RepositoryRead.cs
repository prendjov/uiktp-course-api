using Application.Common.Exceptions;
using Application.Common.Interfaces.Repository.Base;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Persistence.Repository;

public class RepositoryRead<T> : IRepositoryRead<T>
    where T : BaseEntity
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryRead(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetAsync(int id, CancellationToken cancellation = default)
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellation);
    }

    public async Task<T> GetSafeAsync(int id, CancellationToken cancellation = default)
    {
        var entity = await GetAsync(id, cancellation);

        return entity ?? throw NotFoundException.New<T>(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellation = default)
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellation);
    }

    public async Task<IReadOnlyList<T>> GetManyAsync(IEnumerable<int> ids, CancellationToken cancellation = default)
    {
        return await _dbContext.Set<T>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellation);
    }
}
