using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Appearance>> GetAppearances(CancellationToken cancellationToken)
    {
        return await appearanceService.GetAllAppearancesAsync(cancellationToken);
    }

    public async Task<Appearance?> GetAppearance(Guid id, CancellationToken cancellationToken)
    {
        return await appearanceService.GetAppearanceAsync(id, cancellationToken);
    }
}
