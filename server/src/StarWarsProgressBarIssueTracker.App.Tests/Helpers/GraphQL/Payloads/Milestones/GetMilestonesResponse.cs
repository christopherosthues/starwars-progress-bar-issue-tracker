using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;

public class GetMilestonesResponse
{
    public IEnumerable<Milestone> Milestones { get; set; } = [];
}
