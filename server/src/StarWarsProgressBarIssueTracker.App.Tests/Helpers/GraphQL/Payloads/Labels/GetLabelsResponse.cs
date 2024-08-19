using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class GetLabelsResponse
{
    public IEnumerable<Label> Labels { get; set; } = [];
}
