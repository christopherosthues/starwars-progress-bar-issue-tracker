using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;

public class GetMilestoneResponse
{
    public Milestone? Milestone { get; set; }
}
