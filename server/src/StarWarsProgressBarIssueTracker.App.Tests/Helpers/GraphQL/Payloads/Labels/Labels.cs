using StarWarsProgressBarIssueTracker.App.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class Labels
{
    public int TotalCount { get; set; }
    public PageInfo PageInfo { get; set; } = default!;
    public IEnumerable<Edge<LabelDto>> Edges { get; set; } = [];
    public IEnumerable<LabelDto> Nodes { get; set; } = [];
}
