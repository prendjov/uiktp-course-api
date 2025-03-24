using Application.Common.Interfaces;
using Domain.Entities.Base;
using Domain.Interfaces;
using Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _mediator = mediator;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        var result = await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    public void UpdateAuditableEntities()
    {
        if (_dbContext == null) return;

        foreach (var entry in _dbContext.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = (int)_currentUserService.UserId!;
                entry.Entity.Created = _dateTime.Now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = (int)_currentUserService.UserId!;
                entry.Entity.LastModified = _dateTime.Now;
            }
        }
    }
}
