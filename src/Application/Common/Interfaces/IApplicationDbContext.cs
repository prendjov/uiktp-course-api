using Domain.Entities.RefreshTokens;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;
    DbSet<ApplicationUser> User { get; }
    DbSet<RefreshToken> RefreshToken { get; }
}
