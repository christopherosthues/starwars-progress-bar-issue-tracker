using HotChocolate;
using StarWarsProgressBarIssueTracker.App.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class DeleteLabelPayload
{
    public required LabelDto Label { get; set; }

    public Error[] Errors { get; set; } = [];
}
