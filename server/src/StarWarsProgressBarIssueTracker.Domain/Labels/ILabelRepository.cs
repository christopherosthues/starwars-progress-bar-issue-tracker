namespace StarWarsProgressBarIssueTracker.Domain.Labels;

public interface ILabelRepository : IRepository<Label>
{
    IQueryable<Label> GetLabelByIds(IReadOnlyList<Guid> ids);
}
