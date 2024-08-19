using HotChocolate;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Tests.Helpers.GraphQL.Payloads.Labels;

public class DeleteLabelPayload
{
    public required Label Label { get; set; }

    public Error[] Errors { get; set; } = [];
}
