using HotChocolate;
using StarWarsProgressBarIssueTracker.App.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class UpdateLabelPayload
{
    public required LabelDto LabelDto { get; set; }

    public Error[] Errors { get; set; } = [];
}
