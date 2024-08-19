using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Queries;

public partial class IssueTrackerQueries
{
    public async Task<IEnumerable<Milestone>> GetMilestones(CancellationToken cancellationToken)
    {
        return await milestoneService.GetAllMilestonesAsync(cancellationToken);
    }

    public async Task<Milestone?> GetMilestone(Guid id, CancellationToken cancellationToken)
    {
        return await milestoneService.GetMilestoneAsync(id, cancellationToken);
    }
}
