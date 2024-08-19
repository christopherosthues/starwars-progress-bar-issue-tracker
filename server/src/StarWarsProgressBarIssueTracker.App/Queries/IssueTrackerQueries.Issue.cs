using StarWarsProgressBarIssueTracker.Domain.Issues;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Issue>> GetIssues(CancellationToken cancellationToken)
    {
        return await issueService.GetAllIssuesAsync(cancellationToken);
    }

    public async Task<Issue?> GetIssue(Guid id, CancellationToken cancellationToken)
    {
        return await issueService.GetIssueAsync(id, cancellationToken);
    }
}
