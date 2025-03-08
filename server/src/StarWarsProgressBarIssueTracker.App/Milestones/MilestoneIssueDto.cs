using StarWarsProgressBarIssueTracker.App.Commons;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneIssueDto : DtoBase
{
    public required string Title { get; set; }

    public IssueState State { get; set; }

    public Priority Priority { get; set; }

    public MilestoneReleaseDto? Release { get; set; }

    public IList<MilestoneLabelDto> Labels { get; set; } = [];

    public string? Description { get; set; }

    public Vehicle? Vehicle { get; set; }

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
