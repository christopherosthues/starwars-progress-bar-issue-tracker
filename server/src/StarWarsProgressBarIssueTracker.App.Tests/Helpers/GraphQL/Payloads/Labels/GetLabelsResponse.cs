using GraphQL.Types.Relay.DataObjects;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class GetLabelsResponse
{
    public int TotalCount { get; set; }
    public PageInfo PageInfo { get; set; } = default!;
    public IEnumerable<Edge<Label>> Edges { get; set; } = [];
    public IEnumerable<Label> Nodes { get; set; } = [];
}
