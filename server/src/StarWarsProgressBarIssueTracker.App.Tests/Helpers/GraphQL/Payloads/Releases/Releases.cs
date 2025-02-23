using StarWarsProgressBarIssueTracker.App.Releases;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;

public class Releases
{
    public int TotalCount { get; set; }
    public PageInfo PageInfo { get; set; } = null!;
    public IEnumerable<Edge<ReleaseDto>> Edges { get; set; } = [];
    public IEnumerable<ReleaseDto> Nodes { get; set; } = [];
}
