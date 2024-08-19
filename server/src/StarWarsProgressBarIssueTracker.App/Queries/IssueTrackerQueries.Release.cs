using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Release>> GetReleases(CancellationToken cancellationToken)
    {
        return await releaseService.GetAllReleasesAsync(cancellationToken);
    }

    public async Task<Release?> GetRelease(Guid id, CancellationToken cancellationToken)
    {
        return await releaseService.GetReleaseAsync(id, cancellationToken);
    }
}
