using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Domain.Issues;

public interface IIssueRepository : IRepository<Issue>
{
    IQueryable<Issue> GetIssueByIds(IReadOnlyList<Guid> ids);

    void DeleteVehicle(Vehicle dbVehicle);

    void DeleteTranslations(IEnumerable<Translation> dbTranslations);

    void DeletePhotos(IEnumerable<Photo> dbPhotos);

    void DeleteLinks(IEnumerable<IssueLink> dbLinks);
}
