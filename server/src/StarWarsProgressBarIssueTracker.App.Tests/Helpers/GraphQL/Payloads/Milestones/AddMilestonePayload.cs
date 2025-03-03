using HotChocolate;
using StarWarsProgressBarIssueTracker.App.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;

public class AddMilestonePayload
{
    public required MilestoneDto Milestone { get; set; }

    public Error[] Errors { get; set; } = [];
}
