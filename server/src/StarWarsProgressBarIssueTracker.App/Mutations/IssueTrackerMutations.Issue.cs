using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.CodeGen;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DuplicatedAppearanceException>]
    [Error<DuplicatedTranslationsException>]
    [Error<DuplicatedPhotosException>]
    [MutationFieldName(nameof(Issue))]
    public partial async Task<Issue> AddIssue(string title, string? description, Priority priority,
        Guid? milestoneId, Guid? releaseId, Vehicle? vehicle, CancellationToken cancellationToken)
    {
        Milestone? milestone = null;
        if (milestoneId is not null)
        {
            milestone = new Milestone { Id = milestoneId.Value, Title = string.Empty };
        }
        Release? release = null;
        if (releaseId is not null)
        {
            release = new Release { Id = releaseId.Value, Title = string.Empty };
        }

        return await issueService.AddIssueAsync(new()
        {
            Title = title,
            Description = description,
            Milestone = milestone,
            State = IssueState.Open,
            Priority = priority,
            Release = release,
            Vehicle = vehicle
        }, cancellationToken);
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DomainIdNotFoundException>]
    [Error<DuplicatedAppearanceException>]
    [Error<DuplicatedTranslationsException>]
    [Error<DuplicatedPhotosException>]
    [MutationFieldName(nameof(Issue))]
    public partial async Task<Issue> UpdateIssue([ID] Guid id, string title, string? description, Priority priority,
        Guid? milestoneId, Guid? releaseId, Vehicle? vehicle, CancellationToken cancellationToken)
    {
        Milestone? milestone = null;
        if (milestoneId is not null)
        {
            milestone = new Milestone { Id = milestoneId.Value, Title = string.Empty };
        }
        Release? release = null;
        if (releaseId is not null)
        {
            release = new Release { Id = releaseId.Value, Title = string.Empty };
        }

        return await issueService.UpdateIssueAsync(new()
        {
            Id = id,
            Title = title,
            Description = description,
            Milestone = milestone,
            State = IssueState.Open,
            Priority = priority,
            Release = release,
            Vehicle = vehicle
        }, cancellationToken);
    }

    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Issue))]
    public partial async Task<Issue> DeleteIssue([ID] Guid id, CancellationToken cancellationToken)
    {
        return await issueService.DeleteIssueAsync(id, cancellationToken);
    }
}
