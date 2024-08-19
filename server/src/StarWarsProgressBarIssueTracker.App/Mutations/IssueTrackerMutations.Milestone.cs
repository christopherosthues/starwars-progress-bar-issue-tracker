using HotChocolate.Types;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    public async Task<Milestone> AddMilestone(string title, string? description, CancellationToken cancellationToken)
    {
        return await milestoneService.AddMilestoneAsync(new()
        {
            Title = title,
            Description = description,
            State = MilestoneState.Open,
        }, cancellationToken);
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DomainIdNotFoundException>]
    public async Task<Milestone> UpdateMilestone(Guid id, string title, MilestoneState state, string? description, CancellationToken cancellationToken)
    {
        return await milestoneService.UpdateMilestoneAsync(new Milestone
        {
            Id = id,
            Title = title,
            Description = description,
            State = state
        }, cancellationToken);
    }

    [Error<DomainIdNotFoundException>]
    public async Task<Milestone> DeleteMilestone(Guid id, CancellationToken cancellationToken)
    {
        return await milestoneService.DeleteMilestoneAsync(id, cancellationToken);
    }
}
