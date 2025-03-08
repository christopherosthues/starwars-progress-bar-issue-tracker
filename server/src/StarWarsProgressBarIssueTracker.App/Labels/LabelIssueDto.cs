using StarWarsProgressBarIssueTracker.App.Commons;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelIssueDto : DtoBase
{
    public required string Title { get; set; }

    public IssueState State { get; set; }

    public Priority Priority { get; set; }

    public LabelMilestoneDto? Milestone { get; set; }

    public LabelReleaseDto? Release { get; set; }

    public IList<LabelIssueLabelDto> Labels { get; set; } = [];

    public string? Description { get; set; }

    public Vehicle? Vehicle { get; set; }

    public string? GitlabId { get; set; }

    public string? GitlabIid { get; set; }

    public string? GitHubId { get; set; }
}
