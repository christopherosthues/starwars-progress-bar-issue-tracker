using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    public async Task<Issue> AddIssue(string title, string? description, Priority priority,
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

    public async Task<Issue> UpdateIssue(Guid id, string title, string? description, Priority priority,
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

    public async Task<Issue> DeleteIssue(Guid id, CancellationToken cancellationToken)
    {
        return await issueService.DeleteIssueAsync(id, cancellationToken);
    }
}
