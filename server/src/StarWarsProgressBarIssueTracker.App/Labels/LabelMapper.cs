using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelMapper
{
    public LabelDto? MapToNullableLabelDto(Label? label)
    {
        return label == null ? null : MapToLabelDto(label);
    }

    public LabelDto MapToLabelDto(Label label)
    {
        return new LabelDto
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
            Issues = label.Issues.Select(issue => new LabelIssueDto
            {
                Id = issue.Id,
                CreatedAt = issue.CreatedAt,
                LastModifiedAt = issue.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                State = issue.State,
                Priority = issue.Priority,
                Milestone = MapToMilestoneDto(issue.Milestone),
                Release = MapToReleaseDto(issue.Release),
                Vehicle = issue.Vehicle,
                Labels = issue.Labels.Select(MapToIssueLabelDto).ToList(),
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
                GitHubId = issue.GitlabId,
            }).ToList()
        };
    }

    private static LabelMilestoneDto? MapToMilestoneDto(Milestone? milestone)
    {
        if (milestone == null)
        {
            return null;
        }

        return new LabelMilestoneDto
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

    private static LabelReleaseDto? MapToReleaseDto(Release? release)
    {
        if (release == null)
        {
            return null;
        }

        return new LabelReleaseDto
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

    private static LabelIssueLabelDto MapToIssueLabelDto(Label label)
    {
        return new LabelIssueLabelDto
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

    public Label MapToLabel(LabelDto label)
    {
        return new Label
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
            Issues = label.Issues.Select(issue => new Issue
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
