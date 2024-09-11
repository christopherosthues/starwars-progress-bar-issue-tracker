namespace StarWarsProgressBarIssueTracker.Domain.Vehicles;

public interface IAppearanceRepository : IRepository<Appearance>
{
    IQueryable<Appearance> GetAppearanceByIds(IReadOnlyList<Guid> appearanceIds);

    IQueryable<Appearance> GetAppearancesById(IEnumerable<Guid> appearanceIds);
}
