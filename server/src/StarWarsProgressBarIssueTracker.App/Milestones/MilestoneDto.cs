using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneDto : DtoBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public MilestoneState State { get; set; }

    public IList<MilestoneIssueDto> Issues { get; set; } = [];

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
