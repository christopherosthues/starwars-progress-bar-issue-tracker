using HotChocolate.Pagination;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

public class IssueRepository(IssueTrackerContext context) : IssueTrackerRepositoryBase<Issue>(context), IIssueRepository
{
    public override async Task<Page<Issue>> GetAllAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(dbIssue => dbIssue.Milestone)
            .Include(dbIssue => dbIssue.Release)
            .Include(dbIssue => dbIssue.Labels)
            .Include(dbIssue => dbIssue.LinkedIssues)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Appearances)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Photos)
            .Include(dbIssue => dbIssue.Vehicle)
            .ThenInclude(dbVehicle => dbVehicle!.Translations)
            .OrderBy(issue => issue.Title)
            .ThenBy(issue => issue.Id)
            .ToPageAsync(pagingArguments, cancellationToken);
    }

    protected override IQueryable<Issue> GetIncludingFields()
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

    public void DeleteVehicle(Vehicle dbVehicle)
    {
        Context.Vehicles.Remove(dbVehicle);
    }

    public void DeleteTranslations(IEnumerable<Translation> dbTranslations)
    {
        Context.Translations.RemoveRange(dbTranslations);
    }

    public void DeletePhotos(IEnumerable<Photo> dbPhotos)
    {
        Context.Photos.RemoveRange(dbPhotos);
    }

    public void DeleteLinks(IEnumerable<IssueLink> dbLinks)
    {
        Context.IssueLinks.RemoveRange(dbLinks);
    }
}
