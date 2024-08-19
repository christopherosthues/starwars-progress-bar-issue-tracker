namespace StarWarsProgressBarIssueTracker.Domain.Milestones;

public interface IMilestoneService
{
    Task<IEnumerable<Milestone>> GetAllMilestonesAsync(CancellationToken cancellationToken = default);

    Task<Milestone?> GetMilestoneAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default);

    Task<Milestone> UpdateMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default);

    Task<Milestone> DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Milestone> milestones, CancellationToken cancellationToken = default);
}
