using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;

public class Appearances
{
    public int TotalCount { get; set; }
    public PageInfo PageInfo { get; set; } = default!;
    public IEnumerable<Edge<Appearance>> Edges { get; set; } = [];
    public IEnumerable<Appearance> Nodes { get; set; } = [];
}
