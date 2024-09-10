using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface IAppearanceRepository : IRepository<Appearance>
{
    IQueryable<Appearance> GetAppearancesById(IEnumerable<Guid> appearanceIds);
}
