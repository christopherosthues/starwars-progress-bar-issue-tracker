namespace StarWarsProgressBarIssueTracker.App.Releases;

public class ReleaseLabelDto : DtoBase
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public required string Color { get; init; }

    public required string TextColor { get; init; }

    public string? GitlabId { get; init; }

    public string? GitHubId { get; init; }
}
