using GreenDonut.Data;
using StarWarsProgressBarIssueTracker.App.Queries;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;

namespace StarWarsProgressBarIssueTracker.App.Issues;

public class IssueService(IIssueRepository issueRepository, IIssueByIdDataLoader issueByIdDataLoader) : IIssueService
{
    public async Task<Page<Issue>> GetAllIssuesAsync(PagingArguments pagingArguments,
        CancellationToken cancellationToken = default)
    {
        return await issueRepository.GetAllAsync(pagingArguments, cancellationToken);
    }

    public async Task<Issue?> GetIssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await issueByIdDataLoader.LoadAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            throw new DomainIdNotFoundException(nameof(Issue), id.ToString());
        }
    }

    public async Task<Issue> AddIssueAsync(Issue issue, CancellationToken cancellationToken = default)
    {
        ValidateIssue(issue);

        return await issueRepository.AddAsync(issue, cancellationToken);
    }

    private static void ValidateIssue(Issue issue)
    {
        List<Exception> errors = [];
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

            bool hasDuplicatedAppearances = issue.Vehicle.Appearances
                .GroupBy(appearance => appearance.Id).Any(group => group.Count() > 1);
            if (hasDuplicatedAppearances)
            {
                errors.Add(new DuplicatedAppearanceException());
            }

            bool hasDuplicatedTranslations = issue.Vehicle.Translations
                .GroupBy(translation => translation.Country).Any(group => group.Count() > 1);
            // TODO: validate country min and max length and text max length
            if (hasDuplicatedTranslations)
            {
                errors.Add(new DuplicatedTranslationsException());
            }

            bool hasDuplicatedPhotos = issue.Vehicle.Photos
                .GroupBy(photo => photo.FilePath).Any(group => group.Count() > 1);
            // TODO: validate max file path length
            if (hasDuplicatedPhotos)
            {
                errors.Add(new DuplicatedPhotosException());
            }
        }
    }

    public async Task<Issue> UpdateIssueAsync(Issue issue, CancellationToken cancellationToken = default)
    {
        ValidateIssue(issue);

        if (!(await issueRepository.ExistsAsync(issue.Id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Issue), issue.Id.ToString());
        }

        return await issueRepository.UpdateAsync(issue, cancellationToken);
    }

    public async Task<Issue> DeleteIssueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!(await issueRepository.ExistsAsync(id, cancellationToken)))
        {
            throw new DomainIdNotFoundException(nameof(Issue), id.ToString());
        }

        return await issueRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task SynchronizeFromGitlabAsync(IList<Issue> issues, CancellationToken cancellationToken = default)
    {
        // var existingIssues = await issueRepository.GetAllAsync(cancellationToken);
        //
        // var issuesToAdd = issues.Where(issue =>
        //     !existingIssues.Any(existingIssue => issue.GitlabId!.Equals(existingIssue.GitlabId)))
        //     .ToList();
        //
        // var issuesToDelete = existingIssues.Where(existingIssue => existingIssue.GitlabId != null &&
        //                                                            !issues.Any(issue =>
        //                                                                issue.GitlabId!.Equals(existingIssue.GitlabId)));
        //
        // IList<Issue> newIssues = [];
        // foreach (var issueToAdd in issuesToAdd)
        // {
        //     newIssues.Add(new Issue
        //     {
        //         GitlabId = issueToAdd.GitlabId,
        //         GitlabIid = issueToAdd.GitlabIid,
        //         Title = issueToAdd.Title,
        //         Description = issueToAdd.Description,
        //         Priority = issueToAdd.Priority,
        //         Vehicle = issueToAdd.Vehicle != null ?
        //             new Vehicle
        //             {
        //                 EngineColor = issueToAdd.Vehicle.EngineColor,
        //                 Translations = issueToAdd.Vehicle.Translations
        //             } : null
        //     });
        // }
        //
        // await issueRepository.AddRangeAsync(newIssues, cancellationToken);
        //
        // await issueRepository.UpdateRangeByGitlabIdAsync(issuesToAdd, cancellationToken);
        //
        // await issueRepository.DeleteRangeByGitlabIdAsync(issuesToDelete, cancellationToken);

        await Task.CompletedTask;

        // TODO: Update issues, resolve conflicts
    }
}
