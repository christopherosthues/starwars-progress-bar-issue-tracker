using StarWarsProgressBarIssueTracker.App.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Milestones;

public class Milestones
{
    public int TotalCount { get; set; }
    public PageInfo PageInfo { get; set; } = null!;
    public IEnumerable<Edge<MilestoneDto>> Edges { get; set; } = [];
    public IEnumerable<MilestoneDto> Nodes { get; set; } = [];
}
