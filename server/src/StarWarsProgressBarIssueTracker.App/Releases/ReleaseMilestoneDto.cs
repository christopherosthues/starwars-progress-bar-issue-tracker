using StarWarsProgressBarIssueTracker.App.Commons;
using StarWarsProgressBarIssueTracker.Domain.Milestones;

namespace StarWarsProgressBarIssueTracker.App.Releases;

public class ReleaseMilestoneDto : DtoBase
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public MilestoneState State { get; set; }

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
