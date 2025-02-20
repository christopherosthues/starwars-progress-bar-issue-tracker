using StarWarsProgressBarIssueTracker.App.Releases;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;

public class GetReleaseResponse
{
    public ReleaseDto? Release { get; set; }
}
