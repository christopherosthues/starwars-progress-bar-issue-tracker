using HotChocolate;
using StarWarsProgressBarIssueTracker.App.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;

public class DeleteMilestonePayload
{
    public required MilestoneDto Milestone { get; set; }

    public Error[] Errors { get; set; } = [];
}
