using HotChocolate.Authorization;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWarsProgressBarIssueTracker.App.Milestones;
using StarWarsProgressBarIssueTracker.CodeGen.GraphQL;
using StarWarsProgressBarIssueTracker.Domain.Exceptions;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Mutations;

public partial class IssueTrackerMutations
{
    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [MutationFieldName(nameof(Milestone))]
    [Authorize]
    public partial async Task<MilestoneDto> AddMilestone(string title, string? description, CancellationToken cancellationToken)
    {
        return milestoneMapper.MapToMilestoneDto(await milestoneService.AddMilestoneAsync(new()
        {
            Title = title,
            Description = description,
            State = MilestoneState.Open,
        }, cancellationToken));
    }

    [Error<ValueNotSetException>]
    [Error<StringTooShortException>]
    [Error<StringTooLongException>]
    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Milestone))]
    [Authorize]
    public partial async Task<MilestoneDto> UpdateMilestone([ID] Guid id, string title, MilestoneState state, string? description, CancellationToken cancellationToken)
    {
        return milestoneMapper.MapToMilestoneDto(await milestoneService.UpdateMilestoneAsync(new Milestone
        {
            Id = id,
            Title = title,
            Description = description,
            State = state
        }, cancellationToken));
    }

    [Error<DomainIdNotFoundException>]
    [MutationFieldName(nameof(Milestone))]
    [Authorize]
    public partial async Task<MilestoneDto> DeleteMilestone([ID] Guid id, CancellationToken cancellationToken)
    {
        return milestoneMapper.MapToMilestoneDto(await milestoneService.DeleteMilestoneAsync(id, cancellationToken));
    }
}
