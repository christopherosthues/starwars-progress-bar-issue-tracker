using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface IIssueRepository : IRepository<Issue>
{
    void DeleteVehicle(Vehicle dbVehicle);

    void DeleteTranslations(IEnumerable<Translation> dbTranslations);

    void DeletePhotos(IEnumerable<Photo> dbPhotos);

    void DeleteLinks(IEnumerable<IssueLink> dbLinks);
}
