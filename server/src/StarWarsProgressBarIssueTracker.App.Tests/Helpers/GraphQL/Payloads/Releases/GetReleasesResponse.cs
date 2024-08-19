using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;

public class GetReleasesResponse
{
    public IEnumerable<Release> Releases { get; set; } = [];
}
