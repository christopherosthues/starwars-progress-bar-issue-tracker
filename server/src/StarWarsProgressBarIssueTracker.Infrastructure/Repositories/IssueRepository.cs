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
            .ToPageAsync(pagingArguments, true, cancellationToken);
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

    public IQueryable<Issue> GetIssueByIds(IReadOnlyList<Guid> ids)
    {
        return DbSet
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
            .Where(issue => ids.Contains(issue.Id));
    }

    // TODO AddRangeAsync: Load milestones, releases, labels, appearances

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

    // public override async Task<Issue> UpdateAsync(Issue domain, CancellationToken cancellationToken = default)
    // {
    //     DbIssue dbIssue = (await _repository.GetByIdAsync(domain.Id, cancellationToken))!;
    //
    //     dbIssue.Title = domain.Title;
    //     dbIssue.Description = domain.Description;
    //     dbIssue.State = domain.State;
    //     dbIssue.Priority = domain.Priority;
    //
    //     // TODO: update labels
    //
    //     await UpdateIssueMilestoneAsync(domain, dbIssue, cancellationToken);
    //
    //     await UpdateIssueReleaseAsync(domain, dbIssue, cancellationToken);
    //
    //     await UpdateIssueVehicleAsync(domain, dbIssue);
    //
    //     await UpdateIssueLinksAsync(domain, dbIssue, cancellationToken);
    //
    //     DbIssue updatedIssue = await _repository.UpdateAsync(dbIssue, cancellationToken);
    //
    //     return _mapper.Map<Issue>(updatedIssue);
    // }
    //
    // private async Task UpdateIssueMilestoneAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    // {
    //     if (domain.Milestone?.Id != dbIssue.Milestone?.Id)
    //     {
    //         if (domain.Milestone == null)
    //         {
    //             dbIssue.Milestone = null;
    //         }
    //         else
    //         {
    //             DbMilestone? dbMilestone = await _milestoneRepository.GetByIdAsync(domain.Milestone.Id, cancellationToken);
    //             dbIssue.Milestone = dbMilestone;
    //         }
    //     }
    // }
    //
    // private async Task UpdateIssueReleaseAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    // {
    //     if (domain.Release?.Id != dbIssue.Release?.Id)
    //     {
    //         if (domain.Release == null)
    //         {
    //             dbIssue.Release = null;
    //         }
    //         else
    //         {
    //             DbRelease? dbRelease = await _releaseRepository.GetByIdAsync(domain.Release.Id, cancellationToken);
    //             dbIssue.Release = dbRelease;
    //         }
    //     }
    // }
    //
    // private async Task UpdateIssueVehicleAsync(Issue domain, DbIssue dbIssue)
    // {
    //     if (domain.Vehicle == null)
    //     {
    //         if (dbIssue.Vehicle != null)
    //         {
    //             _repository.DeleteVehicle(dbIssue.Vehicle);
    //         }
    //         dbIssue.Vehicle = null;
    //     }
    //     else
    //     {
    //         if (dbIssue.Vehicle != null)
    //         {
    //             var dbVehicle = dbIssue.Vehicle;
    //             dbVehicle.EngineColor = domain.Vehicle.EngineColor;
    //             dbVehicle.Appearances =
    //                 await _appearanceRepository.GetAppearancesById(
    //                     domain.Vehicle.Appearances.Select(appearance => appearance.Id)).ToListAsync();
    //
    //             UpdateTranslations(domain, dbIssue, dbVehicle);
    //
    //             UpdatePhotos(domain, dbIssue, dbVehicle);
    //         }
    //         else
    //         {
    //             var dbVehicle = _mapper.Map<DbVehicle>(domain.Vehicle);
    //             dbVehicle.Appearances =
    //                 await _appearanceRepository.GetAppearancesById(
    //                     domain.Vehicle.Appearances.Select(appearance => appearance.Id)).ToListAsync();
    //             dbIssue.Vehicle = dbVehicle;
    //         }
    //     }
    // }
    //
    // private void UpdateTranslations(Issue domain, DbIssue dbIssue, DbVehicle dbVehicle)
    // {
    //     var addedTranslations = domain.Vehicle!.Translations.Where(translation =>
    //         !dbVehicle.Translations.Any(existingTranslation =>
    //             translation.Country.Equals(existingTranslation.Country)));
    //
    //     var removedTranslations = dbVehicle.Translations.Where(existingTranslation =>
    //         !domain.Vehicle.Translations.Any(translation =>
    //             translation.Country.Equals(existingTranslation.Country)));
    //
    //     var updatedTranslations = domain.Vehicle.Translations.Where(translation =>
    //             dbVehicle.Translations.Any(dbTranslation => translation.Country.Equals(dbTranslation.Country)))
    //         .ToDictionary(translation => translation.Country);
    //
    //     var dbTranslationToUpdate = dbVehicle.Translations.Where(dbTranslation =>
    //             domain.Vehicle.Translations.Any(translation => dbTranslation.Country.Equals(translation.Country)))
    //         .ToList();
    //
    //     _repository.DeleteTranslations(removedTranslations);
    //
    //     foreach (var dbTranslation in dbTranslationToUpdate)
    //     {
    //         dbTranslation.Text = updatedTranslations[dbTranslation.Country].Text;
    //     }
    //
    //     dbIssue.Vehicle!.Translations =
    //         dbTranslationToUpdate.Concat(_mapper.Map<IEnumerable<DbTranslation>>(addedTranslations)).ToList();
    // }
    //
    // private void UpdatePhotos(Issue domain, DbIssue dbIssue, DbVehicle dbVehicle)
    // {
    //     var addedPhotos = domain.Vehicle!.Photos.Where(photo =>
    //         !dbVehicle.Photos.Any(existingPhoto => photo.Id.Equals(existingPhoto.Id)));
    //
    //     var removedPhotos = dbVehicle.Photos.Where(existingPhoto =>
    //         !domain.Vehicle.Photos.Any(photo => photo.Id.Equals(existingPhoto.Id)));
    //
    //     var updatedPhotos = domain.Vehicle.Photos.Where(photo =>
    //             dbVehicle.Photos.Any(dbPhoto => photo.Id.Equals(dbPhoto.Id)))
    //         .ToDictionary(photo => photo.Id);
    //
    //     var dbPhotosToUpdate = dbVehicle.Photos.Where(dbPhoto =>
    //         domain.Vehicle.Photos.Any(photo => dbPhoto.Id.Equals(photo.Id))).ToList();
    //
    //     _repository.DeletePhotos(removedPhotos);
    //
    //     foreach (var dbPhoto in dbPhotosToUpdate)
    //     {
    //         dbPhoto.FilePath = updatedPhotos[dbPhoto.Id].FilePath;
    //     }
    //
    //     dbIssue.Vehicle!.Photos =
    //         dbPhotosToUpdate.Concat(_mapper.Map<IEnumerable<DbPhoto>>(addedPhotos)).ToList();
    // }
    //
    // private async Task UpdateIssueLinksAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    // {
    //     var issueLinks = domain.LinkedIssues;
    //     var dbIssueLinks = dbIssue.LinkedIssues;
    //
    //     var addedLinks = issueLinks.Where(link =>
    //         !dbIssueLinks.Any(dbLink => link.Id.Equals(dbLink.Id)));
    //
    //     var removedLinks = dbIssueLinks.Where(dbLink =>
    //         !issueLinks.Any(link => link.Id.Equals(dbLink.Id)))
    //         .ToList();
    //
    //     foreach (var removedLink in removedLinks)
    //     {
    //         dbIssue.LinkedIssues.Remove(removedLink);
    //     }
    //
    //     _repository.DeleteLinks(removedLinks);
    //
    //     foreach (var addedLink in addedLinks)
    //     {
    //         var addedDbLink = new DbIssueLink
    //         {
    //             Type = addedLink.Type,
    //             LinkedIssue = (await _repository.GetByIdAsync(addedLink.LinkedIssue.Id, cancellationToken))!
    //         };
    //         dbIssue.LinkedIssues.Add(addedDbLink);
    //     }
    // }
    //
    // public async Task UpdateRangeByGitlabIdAsync(IEnumerable<Issue> domains, CancellationToken cancellationToken = default)
    // {
    //     var issueIds = domains.Select(domain => domain.GitlabId!);
    //     var dbIssues = await _repository.GetAll()
    //         .Where(dbIssue => issueIds.Any(domain => domain.Equals(dbIssue.GitlabId)))
    //         .ToListAsync(cancellationToken);
    //     var dbMilestones = await _milestoneRepository.GetAll().ToListAsync(cancellationToken);
    //     var dbReleases = await _releaseRepository.GetAll().ToListAsync(cancellationToken);
    //     var dbAppearances = await _appearanceRepository.GetAll().ToListAsync(cancellationToken);
    //     var dbLabels = await _labelRepository.GetAll().ToListAsync(cancellationToken);
    //
    //     foreach (var domain in domains)
    //     {
    //         var dbIssue = dbIssues.SingleOrDefault(dbIssue => dbIssue.GitlabId?.Equals(domain.GitlabId) ?? false);
    //         if (dbIssue == null)
    //         {
    //             continue;
    //         }
    //
    //         dbIssue.Milestone =
    //             dbMilestones.FirstOrDefault(dbMilestone => dbMilestone.GitlabId?.Equals(domain.Milestone?.GitlabId) ?? false);
    //         dbIssue.Release =
    //             dbReleases.FirstOrDefault(dbRelease => dbRelease.GitlabId?.Equals(domain.Milestone?.GitlabId) ?? false);
    //         dbIssue.Labels = dbLabels.Where(dbLabel =>
    //             domain.Labels.Any(label => label.GitlabId?.Equals(dbLabel.GitlabId) ?? false)).ToList();
    //
    //         if (domain.Vehicle != null && dbIssue.Vehicle != null)
    //         {
    //             dbIssue.Vehicle.Appearances = dbAppearances.Where(dbAppearance =>
    //                 domain.Vehicle.Appearances.Any(appearance =>
    //                     appearance.GitlabId?.Equals(dbAppearance.GitlabId) ?? false)).ToList();
    //         }
    //
    //         foreach (var addedLink in domain.LinkedIssues)
    //         {
    //             var addedDbLink = new DbIssueLink
    //             {
    //                 Type = addedLink.Type,
    //                 LinkedIssue = dbIssues.Single(issue => issue.GitlabId!.Equals(addedLink.LinkedIssue.GitlabId))
    //             };
    //             dbIssue.LinkedIssues.Add(addedDbLink);
    //         }
    //     }
    // }

    // public async Task<Issue> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    // {
    //     DbIssue issue = (await _repository.GetByIdAsync(id, cancellationToken))!;
    //
    //     issue.Milestone = null;
    //     issue.Vehicle?.Appearances.Clear();
    //     issue.Release = null;
    //     issue.Labels.Clear();
    //     // TODO: cascade delete only issue / vehicle related entities. Appearances, Milestones and Labels should stay.
    //     // Vehicle, Translations, Photos, Links should also be deleted
    //
    //     return _mapper.Map<Issue>(await _repository.DeleteAsync(issue, cancellationToken));
    // }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Issue> domains,
        CancellationToken cancellationToken = default)
    {
        // TODO: pasted from data port. Maybe further adjustments needed
        List<Issue> issues = await DbSet.ToListAsync(cancellationToken);
        IEnumerable<Issue> toBeDeleted =
            issues.Where(dbIssue => domains.Any(issue => issue.GitlabId?.Equals(dbIssue.GitlabId) ?? false));
        await DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
