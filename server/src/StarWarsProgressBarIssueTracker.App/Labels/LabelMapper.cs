using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;

namespace StarWarsProgressBarIssueTracker.App.Labels;

public class LabelMapper
{
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
        };
    }

    public GetLabelDto? MapToGetLabelDto(Label? label)
    {
        if (label == null)
        {
            return null;
        }

        return new GetLabelDto
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
                Id = label.Id,
                CreatedAt = label.CreatedAt,
                LastModifiedAt = label.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
                GitHubId = issue.GitlabId,
            }).ToList()
        };
    }

    public Label MapToLabel(GetLabelDto label)
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
                Id = label.Id,
                CreatedAt = label.CreatedAt,
                LastModifiedAt = label.LastModifiedAt,
                Title = issue.Title,
                Description = issue.Description,
                GitlabId = issue.GitlabId,
                GitlabIid = issue.GitlabIid,
                GitHubId = issue.GitlabId,
            }).ToList()
        };
    }
}
