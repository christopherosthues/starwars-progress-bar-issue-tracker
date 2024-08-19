using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;

public class GetReleaseResponse
{
    public required Release? Release { get; set; }
}
