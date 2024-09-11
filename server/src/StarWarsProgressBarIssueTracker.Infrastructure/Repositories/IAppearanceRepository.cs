using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface IAppearanceRepository : IRepository<Appearance>
{
    IQueryable<Appearance> GetAppearanceByIds(IReadOnlyList<Guid> appearanceIds);

    IQueryable<Appearance> GetAppearancesById(IEnumerable<Guid> appearanceIds);
}
