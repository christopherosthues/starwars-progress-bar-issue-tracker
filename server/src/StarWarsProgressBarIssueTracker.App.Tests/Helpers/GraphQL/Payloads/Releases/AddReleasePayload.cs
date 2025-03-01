using HotChocolate;
using StarWarsProgressBarIssueTracker.App.Releases;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Releases;

public class AddReleasePayload
{
    public required ReleaseDto Release { get; set; }

    public Error[] Errors { get; set; } = [];
}
