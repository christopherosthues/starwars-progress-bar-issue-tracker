using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.CodeGen;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [MutationFieldName(nameof(Milestone))]
    public partial async Task<Milestone> AddMilestone(string title, string? description, CancellationToken cancellationToken)
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
    [MutationFieldName(nameof(Milestone))]
    public partial async Task<Milestone> UpdateMilestone([ID] Guid id, string title, MilestoneState state, string? description, CancellationToken cancellationToken)
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
    [MutationFieldName(nameof(Milestone))]
    public partial async Task<Milestone> DeleteMilestone([ID] Guid id, CancellationToken cancellationToken)
    {
        return await milestoneService.DeleteMilestoneAsync(id, cancellationToken);
    }
}
