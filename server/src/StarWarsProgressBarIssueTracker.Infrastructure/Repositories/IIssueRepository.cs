using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public interface IIssueRepository : IRepository<DbIssue>
{
    void DeleteVehicle(DbVehicle dbVehicle);

    void DeleteTranslations(IEnumerable<DbTranslation> dbTranslations);

    void DeletePhotos(IEnumerable<DbPhoto> dbPhotos);

    void DeleteLinks(IEnumerable<DbIssueLink> dbLinks);
}
