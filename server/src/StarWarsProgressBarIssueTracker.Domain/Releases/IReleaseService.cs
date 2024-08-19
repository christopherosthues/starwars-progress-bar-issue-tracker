namespace StarWarsProgressBarIssueTracker.Domain.Releases;

public interface IReleaseService
{
    Task<IEnumerable<Release>> GetAllReleasesAsync(CancellationToken cancellationToken = default);

    Task<Release?> GetReleaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Release> AddReleaseAsync(Release release, CancellationToken cancellationToken = default);

    Task<Release> UpdateReleaseAsync(Release release, CancellationToken cancellationToken = default);

    Task<Release> DeleteReleaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Release> releases, CancellationToken cancellationToken = default);
}
