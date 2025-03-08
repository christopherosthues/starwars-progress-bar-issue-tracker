using StarWarsProgressBarIssueTracker.App.Commons;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneLabelDto : DtoBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Color { get; set; }

    public required string TextColor { get; set; }

    public string? GitlabId { get; set; }

    public string? GitHubId { get; set; }
}
