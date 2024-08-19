using HotChocolate;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;

public class AddAppearancePayload
{
    public required Appearance Appearance { get; set; }

    public Error[] Errors { get; set; } = [];
}
