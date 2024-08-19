using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.Domain.Issues;

public class IssueService(IDataPort<Issue> dataPort) : IIssueService
{
    public async Task<IEnumerable<Issue>> GetAllIssuesAsync(CancellationToken cancellationToken = default)
    {
        return await dataPort.GetAllAsync(cancellationToken);
    }

    public async Task<Issue?> GetIssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dataPort.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default)
    {
        ValidateIssue(issue);

        return await dataPort.AddAsync(issue, cancellationToken);
    }

    private static void ValidateIssue(Issue issue)
    {
        var errors = new List<Exception>();
        if (string.IsNullOrWhiteSpace(issue.Title))
        {
            errors.Add(new ValueNotSetException(nameof(Issue.Title)));
        }

        if (issue.Title.Length < 1)
        {
            errors.Add(new StringTooShortException(issue.Title, nameof(Issue.Title),
                $"The length of {nameof(Issue.Title)} has to be between {IssueConstants.MinTitleLength} and {IssueConstants.MaxTitleLength}."));
        }

        if (issue.Title.Length > IssueConstants.MaxTitleLength)
        {
            errors.Add(new StringTooLongException(issue.Title, nameof(Issue.Title),
                $"The length of {nameof(Issue.Title)} has to be between {IssueConstants.MinTitleLength} and {IssueConstants.MaxTitleLength}."));
        }

        if (issue.Description is not null && issue.Description.Length > IssueConstants.MaxDescriptionLength)
        {
            errors.Add(new StringTooLongException(issue.Description, nameof(Issue.Description),
                $"The length of {nameof(Issue.Description)} has to be less than {IssueConstants.MaxDescriptionLength + 1}."));
        }

        if (!Enum.IsDefined(issue.State) || issue.State == IssueState.Unknown)
        {
            errors.Add(new ValueNotSetException(nameof(Issue.State)));
        }

        if (!Enum.IsDefined(issue.Priority) || issue.Priority == Priority.Unknown)
        {
            errors.Add(new ValueNotSetException(nameof(Issue.Priority)));
        }

        ValidateVehicle(issue, errors);

        if (errors.Count != 0)
        {
            throw new AggregateException(errors);
        }
    }

    private static void ValidateVehicle(Issue issue, List<Exception> errors)
    {
        if (issue.Vehicle != null)
        {
            if (!Enum.IsDefined(issue.Vehicle.EngineColor))
            {
                errors.Add(new ValueNotSetException(nameof(Issue.Vehicle.EngineColor)));
            }

            var hasDuplicatedAppearances = issue.Vehicle.Appearances
                .GroupBy(appearance => appearance.Id).Any(group => group.Count() > 1);
            if (hasDuplicatedAppearances)
            {
                errors.Add(new DuplicatedAppearanceException());
            }

            var hasDuplicatedTranslations = issue.Vehicle.Translations
                .GroupBy(translation => translation.Country).Any(group => group.Count() > 1);
            if (hasDuplicatedTranslations)
            {
                errors.Add(new DuplicatedTranslationsException());
            }

            var hasDuplicatedPhotos = issue.Vehicle.Photos
                .GroupBy(photo => photo.FilePath).Any(group => group.Count() > 1);
            if (hasDuplicatedPhotos)
            {
                errors.Add(new DuplicatedPhotosException());
            }
        }
    }

    public async Task<Issue> UpdateIssueAsync(Issue issue, CancellationToken cancellationToken = default)
    {
        ValidateIssue(issue);

        if (!(await dataPort.ExistsAsync(issue.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Issue), issue.Id.ToString());
        }

        return await dataPort.UpdateAsync(issue, cancellationToken);
    }

    public async Task<Issue> DeleteIssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await dataPort.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Issue), id.ToString());
        }

        return await dataPort.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Issue> issues, CancellationToken cancellationToken = default)
    {
        var existingIssues = await dataPort.GetAllAsync(cancellationToken);

        var issuesToAdd = issues.Where(issue =>
            !existingIssues.Any(existingIssue => issue.GitlabId!.Equals(existingIssue.GitlabId)))
            .ToList();

        var issuesToDelete = existingIssues.Where(existingIssue => existingIssue.GitlabId != null &&
                                                                   !issues.Any(issue =>
                                                                       issue.GitlabId!.Equals(existingIssue.GitlabId)));

        IList<Issue> newIssues = [];
        foreach (var issueToAdd in issuesToAdd)
        {
            newIssues.Add(new Issue
            {
                GitlabId = issueToAdd.GitlabId,
                GitlabIid = issueToAdd.GitlabIid,
                Title = issueToAdd.Title,
                Description = issueToAdd.Description,
                Priority = issueToAdd.Priority,
                Vehicle = issueToAdd.Vehicle != null ?
                    new Vehicle
                    {
                        EngineColor = issueToAdd.Vehicle.EngineColor,
                        Translations = issueToAdd.Vehicle.Translations
                    } : null
            });
        }

        await dataPort.AddRangeAsync(newIssues, cancellationToken);

        await dataPort.UpdateRangeByGitlabIdAsync(issuesToAdd, cancellationToken);

        await dataPort.DeleteRangeByGitlabIdAsync(issuesToDelete, cancellationToken);

        // TODO: Update issues, resolve conflicts
    }
}
