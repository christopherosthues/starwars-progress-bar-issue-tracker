using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface IAppearanceRepository : IRepository<DbAppearance>
{
    IQueryable<DbAppearance> GetAppearancesById(IEnumerable<Guid> appearanceIds);
}
