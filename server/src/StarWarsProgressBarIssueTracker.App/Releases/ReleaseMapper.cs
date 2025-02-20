using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Releases;

public class ReleaseMapper
{
    public ReleaseDto? MapToNullableLabelDto(Release? release)
    {
        return release == null ? null : MapToReleaseDto(release);
    }

    public ReleaseDto MapToReleaseDto(Release release)
    {
        return new ReleaseDto
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            LastModifiedAt = release.LastModifiedAt,
            Title = release.Title,
            Notes = release.Notes,
            Date = release.Date,
            State = release.State,
            GitHubId = release.GitHubId,
            GitlabId = release.GitlabId,
            GitlabIid = release.GitlabIid,
            Issues = release.Issues.Select(issue => new ReleaseIssueDto
            {
                Id = issue.Id,
                CreatedAt = issue.CreatedAt,
                LastModifiedAt = issue.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                State = issue.State,
                Priority = issue.Priority,
                Milestone = MapToMilestoneDto(issue.Milestone),
                Vehicle = issue.Vehicle,
                Labels = issue.Labels.Select(MapToLabelDto).ToList(),
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
                GitHubId = issue.GitlabId,
            }).ToList()
        };
    }

    private static ReleaseMilestoneDto? MapToMilestoneDto(Milestone? milestone)
    {
        if (milestone == null)
        {
            return null;
        }

        return new ReleaseMilestoneDto
        {
            Id = milestone.Id,
            CreatedAt = milestone.CreatedAt,
            LastModifiedAt = milestone.LastModifiedAt,
            Title = milestone.Title,
            Description = milestone.Description,
            State = milestone.State,
            GitlabId = milestone.GitlabId,
            GitlabIid = milestone.GitlabIid,
            GitHubId = milestone.GitHubId,
        };
    }

    private static ReleaseLabelDto MapToLabelDto(Label label)
    {
        return new ReleaseLabelDto
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

    public Release MapToRelease(ReleaseDto release)
    {
        return new Release
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            LastModifiedAt = release.LastModifiedAt,
            Title = release.Title,
            Notes = release.Notes,
            Date = release.Date,
            State = release.State,
            GitHubId = release.GitHubId,
            GitlabId = release.GitlabId,
            Issues = release.Issues.Select(issue => new Issue
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
