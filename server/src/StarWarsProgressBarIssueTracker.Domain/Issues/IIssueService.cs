namespace StarWarsProgressBarIssueTracker.Domain.Issues;

public interface IIssueService
{
    Task<IEnumerable<Issue>> GetAllIssuesAsync(CancellationToken cancellationToken = default);

    Task<Issue?> GetIssueAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default);

    Task<Issue> UpdateIssueAsync(Issue issue, CancellationToken cancellationToken = default);

    Task<Issue> DeleteIssueAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Issue> issues, CancellationToken cancellationToken = default);
}
