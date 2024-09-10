using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public abstract class IssueTrackerRepositoryBase<TDbEntity>(IssueTrackerContext context)
    : IRepository<TDbEntity> where TDbEntity : DomainBase
{
    protected readonly IssueTrackerContext Context = context;

    protected DbSet<TDbEntity> DbSet => Context.Set<TDbEntity>();

    // private IssueTrackerContext? _context;

    // {
        // get => _context ?? throw new InvalidOperationException("The DB context is not initialized.");
        // set
        // {
            // if (_context != null)
            // {
            // throw new InvalidOperationException("THe DB context is already initialized.");
            // }

            // _context = value;
        // }
    // }

    public async Task<TDbEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetIncludingFields().FirstOrDefaultAsync(dbEntity => dbEntity.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(entity => entity.Id.Equals(id), cancellationToken);
    }

    public abstract Task<Page<TDbEntity>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default);
    // {
    //     return await GetIncludingFields()
    //         .AsNoTracking()
    //         .OrderEntities()
    //         .ToListAsync(cancellationToken);
    // }

    protected virtual IQueryable<TDbEntity> GetIncludingFields()
    {
        return DbSet;
    }

    protected virtual IQueryable<TDbEntity> OrderEntities(IQueryable<TDbEntity> queryable)
    {
        return queryable;
    }

    public async Task<TDbEntity> AddAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TDbEntity> resultEntry = await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return resultEntry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TDbEntity> UpdateAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TDbEntity> entry = Context.Entry(entity);
        entry.State = EntityState.Modified;
        await Context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task<TDbEntity> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TDbEntity deletedEntity = (await GetByIdAsync(id, cancellationToken))!;
        DbSet.Remove(deletedEntity);
        await Context.SaveChangesAsync(cancellationToken);
        return deletedEntity;
    }

    public async Task DeleteRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
