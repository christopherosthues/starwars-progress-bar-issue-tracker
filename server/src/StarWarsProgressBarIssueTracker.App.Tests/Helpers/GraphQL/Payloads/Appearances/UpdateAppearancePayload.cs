using HotChocolate;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Appearances;

public class UpdateAppearancePayload
{
    public required Appearance Appearance { get; set; }

    public Error[] Errors { get; set; } = [];
}
