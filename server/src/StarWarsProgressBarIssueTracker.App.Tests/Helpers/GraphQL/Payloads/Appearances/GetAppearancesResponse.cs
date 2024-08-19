using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;

public class GetAppearancesResponse
{
    public IEnumerable<Appearance> Appearances { get; set; } = [];
}
