using HotChocolate.Pagination;

namespace StarWarsProgressBarIssueTracker.Domain.Milestones;

public interface IMilestoneService
{
    Task<Page<Milestone>> GetAllMilestonesAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default);

    Task<Milestone?> GetMilestoneAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Milestone> AddMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default);

    Task<Milestone> UpdateMilestoneAsync(Milestone milestone, CancellationToken cancellationToken = default);

    Task<Milestone> DeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default);

    Task SynchronizeFromGitlabAsync(IList<Milestone> milestones, CancellationToken cancellationToken = default);
}
