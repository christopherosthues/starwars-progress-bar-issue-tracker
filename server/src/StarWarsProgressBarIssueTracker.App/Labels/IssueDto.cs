using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class IssueDto : DtoBase
{
    public required string Title { get; set; }

    public IssueState State { get; set; }

    public Priority Priority { get; set; }

    public MilestoneDto? Milestone { get; set; }

    public ReleaseDto? Release { get; set; }

    public IList<IssueLabelDto> Labels { get; set; } = [];

    public string? Description { get; set; }

    public Vehicle? Vehicle { get; set; }

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}