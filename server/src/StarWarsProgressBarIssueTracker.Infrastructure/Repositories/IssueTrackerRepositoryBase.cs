using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class IssueTrackerRepositoryBase<TDbEntity> : IRepository<TDbEntity> where TDbEntity : DbEntityBase
{
    protected DbSet<TDbEntity> DbSet => Context.Set<TDbEntity>();

    private IssueTrackerContext? _context;

    public IssueTrackerContext Context
    {
        get => _context ?? throw new InvalidOperationException("The DB context is not initialized.");
        set
        {
            // if (_context != null)
            // {
            // throw new InvalidOperationException("THe DB context is already initialized.");
            // }

            _context = value;
        }
    }

    public async Task<TDbEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetIncludingFields().FirstOrDefaultAsync(dbEntity => dbEntity.Id.Equals(id), cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(entity => entity.Id.Equals(id), cancellationToken);
    }

    public IQueryable<TDbEntity> GetAll()
    {
        return GetIncludingFields();
    }

    protected virtual IQueryable<TDbEntity> GetIncludingFields()
    {
        return DbSet;
    }

    public async Task<TDbEntity> AddAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        var resultEntry = await DbSet.AddAsync(entity, cancellationToken);
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
        var entry = Context.Entry(entity);
        entry.State = EntityState.Modified;
        await Context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public async Task<TDbEntity> DeleteAsync(TDbEntity entity, CancellationToken cancellationToken = default)
    {
        var deletedEntity = DbSet.Remove(entity).Entity;
        await Context.SaveChangesAsync(cancellationToken);
        return deletedEntity;
    }

    public async Task DeleteRangeAsync(IEnumerable<TDbEntity> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await Context.SaveChangesAsync(cancellationToken);
    }
}
