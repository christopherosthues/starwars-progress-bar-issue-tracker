using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class ReleaseRepository : IssueTrackerRepositoryBase<DbRelease>
{
    protected override IQueryable<DbRelease> GetIncludingFields()
    {
        return DbSet.Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Milestone)
            .Include(dbRelease => dbRelease.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances);
    }
}
