namespace StarWarsProgressBarIssueTracker.Domain.Releases;

public interface IReleaseRepository : IRepository<Release>
{
    IQueryable<Release> GetReleaseByIds(IReadOnlyList<Guid> ids);
}
