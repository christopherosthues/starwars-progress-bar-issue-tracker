using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Domain;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

namespace StarWarsProgressBarIssueTracker.App.Issues;

public class IssueDataPort : IDataPort<Issue>
{
    private readonly IIssueRepository _repository;
    private readonly IAppearanceRepository _appearanceRepository;
    private readonly IRepository<DbLabel> _labelRepository;
    private readonly IRepository<DbMilestone> _milestoneRepository;
    private readonly IRepository<DbRelease> _releaseRepository;
    private readonly IMapper _mapper;

    public IssueDataPort(IssueTrackerContext context,
        IIssueRepository repository,
        IAppearanceRepository appearanceRepository,
        IRepository<DbMilestone> milestoneRepository,
        IRepository<DbRelease> releaseRepository,
        IRepository<DbLabel> labelRepository,
        IMapper mapper)
    {
        _repository = repository;
        _appearanceRepository = appearanceRepository;
        _milestoneRepository = milestoneRepository;
        _releaseRepository = releaseRepository;
        _labelRepository = labelRepository;
        _repository.Context = context;
        _appearanceRepository.Context = context;
        _milestoneRepository.Context = context;
        _releaseRepository.Context = context;
        _labelRepository.Context = context;
        _mapper = mapper;
    }

    public async Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbIssue? dbIssue = await _repository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<Issue?>(dbIssue);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Issue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<DbIssue> dbIssues = await _repository.GetAll().ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<Issue>>(dbIssues);
    }

    public async Task<Issue> AddAsync(Issue domain, CancellationToken cancellationToken = default)
    {
        var addedDbIssue = await _repository.AddAsync(_mapper.Map<DbIssue>(domain), cancellationToken);

        return _mapper.Map<Issue>(addedDbIssue);
    }

    public async Task AddRangeAsync(IEnumerable<Issue> domains, CancellationToken cancellationToken = default)
    {
        var dbIssues = _mapper.Map<IEnumerable<DbIssue>>(domains);
        // TODO: Load milestones, releases, labels, appearances
        await _repository.AddRangeAsync(dbIssues, cancellationToken);
    }

    public async Task<Issue> UpdateAsync(Issue domain, CancellationToken cancellationToken = default)
    {
        DbIssue dbIssue = (await _repository.GetByIdAsync(domain.Id, cancellationToken))!;

        dbIssue.Title = domain.Title;
        dbIssue.Description = domain.Description;
        dbIssue.State = domain.State;
        dbIssue.Priority = domain.Priority;

        // TODO: update labels

        await UpdateIssueMilestoneAsync(domain, dbIssue, cancellationToken);

        await UpdateIssueReleaseAsync(domain, dbIssue, cancellationToken);

        await UpdateIssueVehicleAsync(domain, dbIssue);

        await UpdateIssueLinksAsync(domain, dbIssue, cancellationToken);

        DbIssue updatedIssue = await _repository.UpdateAsync(dbIssue, cancellationToken);

        return _mapper.Map<Issue>(updatedIssue);
    }

    private async Task UpdateIssueMilestoneAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    {
        if (domain.Milestone?.Id != dbIssue.Milestone?.Id)
        {
            if (domain.Milestone == null)
            {
                dbIssue.Milestone = null;
            }
            else
            {
                DbMilestone? dbMilestone = await _milestoneRepository.GetByIdAsync(domain.Milestone.Id, cancellationToken);
                dbIssue.Milestone = dbMilestone;
            }
        }
    }

    private async Task UpdateIssueReleaseAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    {
        if (domain.Release?.Id != dbIssue.Release?.Id)
        {
            if (domain.Release == null)
            {
                dbIssue.Release = null;
            }
            else
            {
                DbRelease? dbRelease = await _releaseRepository.GetByIdAsync(domain.Release.Id, cancellationToken);
                dbIssue.Release = dbRelease;
            }
        }
    }

    private async Task UpdateIssueVehicleAsync(Issue domain, DbIssue dbIssue)
    {
        if (domain.Vehicle == null)
        {
            if (dbIssue.Vehicle != null)
            {
                _repository.DeleteVehicle(dbIssue.Vehicle);
            }
            dbIssue.Vehicle = null;
        }
        else
        {
            if (dbIssue.Vehicle != null)
            {
                var dbVehicle = dbIssue.Vehicle;
                dbVehicle.EngineColor = domain.Vehicle.EngineColor;
                dbVehicle.Appearances =
                    await _appearanceRepository.GetAppearancesById(
                        domain.Vehicle.Appearances.Select(appearance => appearance.Id)).ToListAsync();

                UpdateTranslations(domain, dbIssue, dbVehicle);

                UpdatePhotos(domain, dbIssue, dbVehicle);
            }
            else
            {
                var dbVehicle = _mapper.Map<DbVehicle>(domain.Vehicle);
                dbVehicle.Appearances =
                    await _appearanceRepository.GetAppearancesById(
                        domain.Vehicle.Appearances.Select(appearance => appearance.Id)).ToListAsync();
                dbIssue.Vehicle = dbVehicle;
            }
        }
    }

    private void UpdateTranslations(Issue domain, DbIssue dbIssue, DbVehicle dbVehicle)
    {
        var addedTranslations = domain.Vehicle!.Translations.Where(translation =>
            !dbVehicle.Translations.Any(existingTranslation =>
                translation.Country.Equals(existingTranslation.Country)));

        var removedTranslations = dbVehicle.Translations.Where(existingTranslation =>
            !domain.Vehicle.Translations.Any(translation =>
                translation.Country.Equals(existingTranslation.Country)));

        var updatedTranslations = domain.Vehicle.Translations.Where(translation =>
                dbVehicle.Translations.Any(dbTranslation => translation.Country.Equals(dbTranslation.Country)))
            .ToDictionary(translation => translation.Country);

        var dbTranslationToUpdate = dbVehicle.Translations.Where(dbTranslation =>
                domain.Vehicle.Translations.Any(translation => dbTranslation.Country.Equals(translation.Country)))
            .ToList();

        _repository.DeleteTranslations(removedTranslations);

        foreach (var dbTranslation in dbTranslationToUpdate)
        {
            dbTranslation.Text = updatedTranslations[dbTranslation.Country].Text;
        }

        dbIssue.Vehicle!.Translations =
            dbTranslationToUpdate.Concat(_mapper.Map<IEnumerable<DbTranslation>>(addedTranslations)).ToList();
    }

    private void UpdatePhotos(Issue domain, DbIssue dbIssue, DbVehicle dbVehicle)
    {
        var addedPhotos = domain.Vehicle!.Photos.Where(photo =>
            !dbVehicle.Photos.Any(existingPhoto => photo.Id.Equals(existingPhoto.Id)));

        var removedPhotos = dbVehicle.Photos.Where(existingPhoto =>
            !domain.Vehicle.Photos.Any(photo => photo.Id.Equals(existingPhoto.Id)));

        var updatedPhotos = domain.Vehicle.Photos.Where(photo =>
                dbVehicle.Photos.Any(dbPhoto => photo.Id.Equals(dbPhoto.Id)))
            .ToDictionary(photo => photo.Id);

        var dbPhotosToUpdate = dbVehicle.Photos.Where(dbPhoto =>
            domain.Vehicle.Photos.Any(photo => dbPhoto.Id.Equals(photo.Id))).ToList();

        _repository.DeletePhotos(removedPhotos);

        foreach (var dbPhoto in dbPhotosToUpdate)
        {
            dbPhoto.FilePath = updatedPhotos[dbPhoto.Id].FilePath;
        }

        dbIssue.Vehicle!.Photos =
            dbPhotosToUpdate.Concat(_mapper.Map<IEnumerable<DbPhoto>>(addedPhotos)).ToList();
    }

    private async Task UpdateIssueLinksAsync(Issue domain, DbIssue dbIssue, CancellationToken cancellationToken)
    {
        var issueLinks = domain.LinkedIssues;
        var dbIssueLinks = dbIssue.LinkedIssues;

        var addedLinks = issueLinks.Where(link =>
            !dbIssueLinks.Any(dbLink => link.Id.Equals(dbLink.Id)));

        var removedLinks = dbIssueLinks.Where(dbLink =>
            !issueLinks.Any(link => link.Id.Equals(dbLink.Id)))
            .ToList();

        foreach (var removedLink in removedLinks)
        {
            dbIssue.LinkedIssues.Remove(removedLink);
        }

        _repository.DeleteLinks(removedLinks);

        foreach (var addedLink in addedLinks)
        {
            var addedDbLink = new DbIssueLink
            {
                Type = addedLink.Type,
                LinkedIssue = (await _repository.GetByIdAsync(addedLink.LinkedIssue.Id, cancellationToken))!
            };
            dbIssue.LinkedIssues.Add(addedDbLink);
        }
    }

    public async Task UpdateRangeByGitlabIdAsync(IEnumerable<Issue> domains, CancellationToken cancellationToken = default)
    {
        var issueIds = domains.Select(domain => domain.GitlabId!);
        var dbIssues = await _repository.GetAll()
            .Where(dbIssue => issueIds.Any(domain => domain.Equals(dbIssue.GitlabId)))
            .ToListAsync(cancellationToken);
        var dbMilestones = await _milestoneRepository.GetAll().ToListAsync(cancellationToken);
        var dbReleases = await _releaseRepository.GetAll().ToListAsync(cancellationToken);
        var dbAppearances = await _appearanceRepository.GetAll().ToListAsync(cancellationToken);
        var dbLabels = await _labelRepository.GetAll().ToListAsync(cancellationToken);

        foreach (var domain in domains)
        {
            var dbIssue = dbIssues.SingleOrDefault(dbIssue => dbIssue.GitlabId?.Equals(domain.GitlabId) ?? false);
            if (dbIssue == null)
            {
                continue;
            }

            dbIssue.Milestone =
                dbMilestones.FirstOrDefault(dbMilestone => dbMilestone.GitlabId?.Equals(domain.Milestone?.GitlabId) ?? false);
            dbIssue.Release =
                dbReleases.FirstOrDefault(dbRelease => dbRelease.GitlabId?.Equals(domain.Milestone?.GitlabId) ?? false);
            dbIssue.Labels = dbLabels.Where(dbLabel =>
                domain.Labels.Any(label => label.GitlabId?.Equals(dbLabel.GitlabId) ?? false)).ToList();

            if (domain.Vehicle != null && dbIssue.Vehicle != null)
            {
                dbIssue.Vehicle.Appearances = dbAppearances.Where(dbAppearance =>
                    domain.Vehicle.Appearances.Any(appearance =>
                        appearance.GitlabId?.Equals(dbAppearance.GitlabId) ?? false)).ToList();
            }

            foreach (var addedLink in domain.LinkedIssues)
            {
                var addedDbLink = new DbIssueLink
                {
                    Type = addedLink.Type,
                    LinkedIssue = dbIssues.Single(issue => issue.GitlabId!.Equals(addedLink.LinkedIssue.GitlabId))
                };
                dbIssue.LinkedIssues.Add(addedDbLink);
            }
        }
    }

    public async Task<Issue> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DbIssue issue = (await _repository.GetByIdAsync(id, cancellationToken))!;

        issue.Milestone = null;
        issue.Vehicle?.Appearances.Clear();
        issue.Release = null;
        issue.Labels.Clear();
        // TODO: cascade delete only issue / vehicle related entities. Appearances, Milestones and Labels should stay.
        // Vehicle, Translations, Photos, Links should also be deleted

        return _mapper.Map<Issue>(await _repository.DeleteAsync(issue, cancellationToken));
    }

    public async Task DeleteRangeByGitlabIdAsync(IEnumerable<Issue> domains, CancellationToken cancellationToken = default)
    {
        var issues = await _repository.GetAll().ToListAsync(cancellationToken);
        var toBeDeleted = issues.Where(dbIssue => domains.Any(issue => issue.GitlabId?.Equals(dbIssue.GitlabId) ?? false));
        await _repository.DeleteRangeAsync(toBeDeleted, cancellationToken);
    }
}
