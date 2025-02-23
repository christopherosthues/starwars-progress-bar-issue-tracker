using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class MilestoneRepository(IssueTrackerContext context)
    : IssueTrackerRepositoryBase<Milestone>(context), IMilestoneRepository
{
    public override async Task<List<Milestone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .OrderBy(milestone => milestone.Title)
            .ThenBy(milestone => milestone.Id)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Page<Milestone>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .OrderBy(milestone => milestone.Title)
            .ThenBy(milestone => milestone.Id)
            .ToPageAsync(pagingArguments, true, cancellationToken);
    }

    public IQueryable<Milestone> GetMilestoneByIds(IReadOnlyList<Guid> ids)
    {
        return DbSet
            .AsNoTracking()
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .Where(dbMilestone => ids.Contains(dbMilestone.Id));
    }

    protected override IQueryable<Milestone> GetIncludingFields()
    {
        return DbSet.Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release);
    }

    public override async Task<Milestone> UpdateAsync(Milestone entity, CancellationToken cancellationToken = default)
    {
        Milestone dbEntity = (await GetByIdAsync(entity.Id, cancellationToken))!;
        dbEntity.Title = entity.Title;
        dbEntity.Description = entity.Description;
        dbEntity.State = entity.State;
        await Context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    // TODO: override Delete and clear issues

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Milestone> domains,
        CancellationToken cancellationToken = default)
    {
        // TODO: pasted from data port. Maybe further adjustments needed
        List<Milestone> milestones = await DbSet.ToListAsync(cancellationToken);
        IEnumerable<Milestone> toBeDeleted = milestones.Where(dbMilestone =>
            domains.Any(milestone => milestone.GitlabId?.Equals(dbMilestone.GitlabId) ?? false));
        await DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
