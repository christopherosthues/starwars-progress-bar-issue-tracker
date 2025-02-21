using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class ReleaseRepository(IssueTrackerContext context)
    : IssueTrackerRepositoryBase<Release>(context), IReleaseRepository
{
    public override async Task<Page<Release>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .OrderBy(release => release.Title)
            .ThenBy(release => release.Id)
            .ToPageAsync(pagingArguments, true, cancellationToken);
    }

    public IQueryable<Release> GetReleaseByIds(IReadOnlyList<Guid> ids)
    {
        return DbSet
            .AsNoTracking()
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Where(label => ids.Contains(label.Id));
    }

    protected override IQueryable<Release> GetIncludingFields()
    {
        return DbSet.Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances);
    }

    public override async Task<Release> UpdateAsync(Release entity, CancellationToken cancellationToken = default)
    {
        Release dbEntity = (await GetByIdAsync(entity.Id, cancellationToken))!;
        dbEntity.Title = entity.Title;
        dbEntity.Notes = entity.Notes;
        dbEntity.Date = entity.Date;
        dbEntity.State = entity.State;
        await Context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Release> domains, CancellationToken cancellationToken = default)
    {
        // TODO: pasted from data port. Maybe further adjustments needed
        List<Release> releases = await DbSet.ToListAsync(cancellationToken);
        IEnumerable<Release> toBeDeleted = releases.Where(dbRelease => domains.Any(release => release.GitlabId?.Equals(dbRelease.GitlabId) ?? false));
        await DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
