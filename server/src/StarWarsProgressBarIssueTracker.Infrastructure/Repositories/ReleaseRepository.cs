using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class ReleaseRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<Release>(context)
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
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    protected override IQueryable<Release> GetIncludingFields()
    {
        return DbSet.Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances);
    }
}
