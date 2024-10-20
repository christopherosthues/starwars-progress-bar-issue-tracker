using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public abstract class IssueTrackerRepositoryBase<TDomain>(IssueTrackerContext context)
    : IRepository<TDomain> where TDomain : DomainBase
{
    protected readonly IssueTrackerContext Context = context;

    protected DbSet<TDomain> DbSet => Context.Set<TDomain>();

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

    public async Task<TDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetIncludingFields().FirstOrDefaultAsync(dbEntity => dbEntity.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(entity => entity.Id.Equals(id), cancellationToken);
    }

    public abstract Task<Page<TDomain>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default);

    protected virtual IQueryable<TDomain> GetIncludingFields()
    {
        return DbSet;
    }

    public async Task<TDomain> AddAsync(TDomain entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TDomain> resultEntry = await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return resultEntry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<TDomain> UpdateAsync(TDomain entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<TDomain> entry = Context.Entry(entity);
        entry.State = EntityState.Modified;
        await Context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task<TDomain> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TDomain deletedEntity = (await GetByIdAsync(id, cancellationToken))!;
        DbSet.Remove(deletedEntity);
        await Context.SaveChangesAsync(cancellationToken);
        return deletedEntity;
    }

    public async Task DeleteRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
