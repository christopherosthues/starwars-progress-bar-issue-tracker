using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class IssueRepository : IssueTrackerRepositoryBase<DbIssue>, IIssueRepository
{
    protected override IQueryable<DbIssue> GetIncludingFields()
    {
        return Context.Issues.Include(dbIssue => dbIssue.Milestone)
            .Include(dbIssue => dbIssue.Release)
            .Include(dbIssue => dbIssue.Labels)
            .Include(dbIssue => dbIssue.LinkedIssues)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Photos)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Translations);
    }

    public void DeleteVehicle(DbVehicle dbVehicle)
    {
        Context.Vehicles.Remove(dbVehicle);
    }

    public void DeleteTranslations(IEnumerable<DbTranslation> dbTranslations)
    {
        Context.Translations.RemoveRange(dbTranslations);
    }

    public void DeletePhotos(IEnumerable<DbPhoto> dbPhotos)
    {
        Context.Photos.RemoveRange(dbPhotos);
    }

    public void DeleteLinks(IEnumerable<DbIssueLink> dbLinks)
    {
        Context.IssueLinks.RemoveRange(dbLinks);
    }
}
