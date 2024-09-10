using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class MilestoneRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<Milestone>(context)
{
    public override async Task<Page<Milestone>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release)
            .OrderBy(milestone => milestone.Title)
            .ThenBy(milestone => milestone.Id)
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    protected override IQueryable<Milestone> GetIncludingFields()
    {
        return DbSet.Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbMilestone => dbMilestone.Issues)
            .ThenInclude(dbIssue => dbIssue.Release);
    }
}
