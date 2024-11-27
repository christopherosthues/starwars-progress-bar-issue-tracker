namespace StarWarsProgressBarIssueTracker.Domain.Milestones;

public interface IMilestoneRepository : IRepository<Milestone>
{
    IQueryable<Milestone> GetMilestoneByIds(IReadOnlyList<Guid> ids);
}
