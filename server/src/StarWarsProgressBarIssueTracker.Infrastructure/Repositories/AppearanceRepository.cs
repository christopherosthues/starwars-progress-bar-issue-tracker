using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class AppearanceRepository : IssueTrackerRepositoryBase<DbAppearance>, IAppearanceRepository
{
    public IQueryable<DbAppearance> GetAppearancesById(IEnumerable<Guid> appearanceIds)
    {
        return DbSet.Where(dbAppearance => appearanceIds.Any(id => id.Equals(dbAppearance.Id)));
    }
}
