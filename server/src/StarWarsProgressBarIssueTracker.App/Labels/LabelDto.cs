
using StarWarsProgressBarIssueTracker.App.Commons;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelDto : DtoBase
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public required string Color { get; init; }

    public required string TextColor { get; init; }

    public IList<LabelIssueDto> Issues { get; init; } = [];

    public string? GitlabId { get; init; }

    public string? GitHubId { get; init; }
}
