using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Milestones;

public class MilestoneMapper
{
    public MilestoneDto? MapToNullableMilestoneDto(Milestone? milestone)
    {
        return milestone == null ? null : MapToMilestoneDto(milestone);
    }

    public MilestoneDto MapToMilestoneDto(Milestone milestone)
    {
        return new MilestoneDto
        {
            Id = milestone.Id,
            CreatedAt = milestone.CreatedAt,
            LastModifiedAt = milestone.LastModifiedAt,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            GitHubId = milestone.GitHubId,
            GitlabId = milestone.GitlabId,
            GitlabIid = milestone.GitlabIid,
            Issues = milestone.Issues.Select(issue => new MilestoneIssueDto
            {
                Id = issue.Id,
                CreatedAt = issue.CreatedAt,
                LastModifiedAt = issue.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                State = issue.State,
                Priority = issue.Priority,
                Release = MapToReleaseDto(issue.Release),
                Vehicle = issue.Vehicle,
                Labels = issue.Labels.Select(MapToLabelDto).ToList(),
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
                GitHubId = issue.GitlabId,
            }).ToList()
        };
    }

    private static MilestoneReleaseDto? MapToReleaseDto(Release? release)
    {
        if (release == null)
        {
            return null;
        }

        return new MilestoneReleaseDto
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            LastModifiedAt = release.LastModifiedAt,
            Title = release.Title,
            Notes = release.Notes,
            State = release.State,
            Date = release.Date,
            GitlabId = release.GitlabId,
            GitlabIid = release.GitlabIid,
            GitHubId = release.GitlabId,
        };
    }

    private static MilestoneLabelDto MapToLabelDto(Label label)
    {
        return new MilestoneLabelDto
        {
            Id = label.Id,
            CreatedAt = label.CreatedAt,
            LastModifiedAt = label.LastModifiedAt,
            Title = label.Title,
            Description = label.Description,
            Color = label.Color,
            TextColor = label.TextColor,
            GitHubId = label.GitHubId,
            GitlabId = label.GitlabId,
        };
    }

    public Milestone MapToMilestone(MilestoneDto milestone)
    {
        return new Milestone
        {
            Id = milestone.Id,
            CreatedAt = milestone.CreatedAt,
            LastModifiedAt = milestone.LastModifiedAt,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            GitHubId = milestone.GitHubId,
            GitlabId = milestone.GitlabId,
            GitlabIid = milestone.GitlabIid,
            Issues = milestone.Issues.Select(issue => new Issue
            {
                Id = issue.Id,
                CreatedAt = issue.CreatedAt,
                LastModifiedAt = issue.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                State = issue.State,
                Priority = issue.Priority,
                GitHubId = issue.GitlabIid,
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
            }).ToList()
        };
    }
}
