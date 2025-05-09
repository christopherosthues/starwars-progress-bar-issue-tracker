using System.Text.Json;
using StarWarsProgressBarIssueTracker.Domain.Issues;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Milestones;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Domain.Vehicles;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Models;
using IssueState = StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL.IssueState;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Services;

public class GitlabSynchronizationService(GraphQLService graphQlService,
    RestService restService,
    IAppearanceService appearanceService,
    ILabelService labelService,
    IMilestoneService milestoneService,
    IIssueService issueService,
    IReleaseService releaseService)
{
    private const string ReleaseTitle = "release";
    private const string AppearanceMarker = "appearance:";

    public async Task SynchronizeAsync(CancellationToken cancellationToken)
    {
        IGetAll_Project? all = await graphQlService.GetAllAsync(cancellationToken);

        if (all == null)
        {
            return;
        }

        await SynchronizeAppearancesAsync(all.Labels, cancellationToken);

        await SynchronizeMilestonesAsync(all.Milestones, cancellationToken);

        await SynchronizeIssuesAndReleasesAsync(all.Id.Split('/')[^1], all.Issues, cancellationToken);
    }

    private async Task SynchronizeAppearancesAsync(IGetAll_Project_Labels? gitlabLabelData,
        CancellationToken cancellationToken)
    {
        IEnumerable<IGetAll_Project_Labels_Nodes> gitlabLabels = (gitlabLabelData?.Nodes ?? [])
            .Where(gitlabLabel => gitlabLabel != null).Cast<IGetAll_Project_Labels_Nodes>();
        IList<Label> labels = [];
        IList<Appearance> appearances = [];
        foreach (IGetAll_Project_Labels_Nodes gitlabLabel in gitlabLabels)
        {
            if (gitlabLabel.Title.StartsWith("appearance: ", StringComparison.InvariantCultureIgnoreCase))
            {
                appearances.Add(new Appearance
                {
                    GitlabId = gitlabLabel.Id,
                    Title = gitlabLabel.Title["Appearance: ".Length..],
                    Description = gitlabLabel.Description,
                    Color = gitlabLabel.Color,
                    TextColor = gitlabLabel.TextColor,
                    LastModifiedAt = DateTime.Parse(gitlabLabel.UpdatedAt).ToUniversalTime()
                });
            }
            else
            {
                labels.Add(new Label
                {
                    GitlabId = gitlabLabel.Id,
                    Title = gitlabLabel.Title,
                    Description = gitlabLabel.Description,
                    Color = gitlabLabel.Color,
                    TextColor = gitlabLabel.TextColor,
                    LastModifiedAt = DateTime.Parse(gitlabLabel.UpdatedAt).ToUniversalTime()
                });
            }
        }

        await appearanceService.SynchronizeFromGitlabAsync(appearances, cancellationToken);
        await labelService.SynchronizeFromGitlabAsync(labels, cancellationToken);
    }

    private async Task SynchronizeMilestonesAsync(IGetAll_Project_Milestones? gitlabMilestoneData,
        CancellationToken cancellationToken)
    {
        IEnumerable<IGetAll_Project_Milestones_Nodes> gitlabMilestones = (gitlabMilestoneData?.Nodes ?? [])
            .Where(gitlabMilestone => gitlabMilestone != null).Cast<IGetAll_Project_Milestones_Nodes>();
        IList<Milestone> milestones = gitlabMilestones.Select(gitlabMilestone =>
            new Milestone
            {
                GitlabId = gitlabMilestone.Id,
                GitlabIid = gitlabMilestone.Iid,
                Title = gitlabMilestone.Title,
                Description = gitlabMilestone.Description,
                State = MapMilestoneState(gitlabMilestone.State),
                LastModifiedAt = DateTime.Parse(gitlabMilestone.UpdatedAt).ToUniversalTime()
            }).ToList();

        await milestoneService.SynchronizeFromGitlabAsync(milestones, cancellationToken);
    }

    private static MilestoneState MapMilestoneState(MilestoneStateEnum milestoneState) =>
        milestoneState switch
        {
            MilestoneStateEnum.Active => MilestoneState.Open,
            MilestoneStateEnum.Closed => MilestoneState.Closed,
            _ => MilestoneState.Closed
        };

    private async Task SynchronizeIssuesAndReleasesAsync(string projectId, IGetAll_Project_Issues? gitlabIssueData,
        CancellationToken cancellationToken)
    {
        IGetAll_Project_Issues_PageInfo? currentPageInfo = gitlabIssueData?.PageInfo;

        List<IGetFurtherIssues_Project_Issues_Nodes> gitlabFurtherIssues = await LoadAllRemainingIssues(currentPageInfo, cancellationToken);

        IEnumerable<IGetAll_Project_Issues_Nodes> gitlabFirstIssues = (gitlabIssueData?.Nodes ?? [])
            .Where(issue => issue != null).Cast<IGetAll_Project_Issues_Nodes>();
        IList<Issue> issues = [];
        IList<Release> releases = [];
        await ProcessFirst100IssuesAsync(int.Parse(projectId), gitlabFirstIssues, releases, issues);
        await ProcessRemainingIssuesAsync(int.Parse(projectId), gitlabFurtherIssues, releases, issues);

        await releaseService.SynchronizeFromGitlabAsync(releases, cancellationToken);
        await issueService.SynchronizeFromGitlabAsync(issues, cancellationToken);
    }

    private async Task ProcessFirst100IssuesAsync(int projectId,
        IEnumerable<IGetAll_Project_Issues_Nodes> gitlabFirstIssues,
        ICollection<Release> releases, ICollection<Issue> issues)
    {
        foreach (IGetAll_Project_Issues_Nodes gitlabIssue in gitlabFirstIssues)
        {
            if (gitlabIssue.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
            {
                releases.Add(new Release
                {
                    GitlabId = gitlabIssue.Id,
                    GitlabIid = gitlabIssue.Iid,
                    Title = gitlabIssue.Title,
                    Notes = gitlabIssue.Description,
                    State = MapReleaseState(gitlabIssue.State),
                    Date = gitlabIssue.DueDate != null
                        ? DateTime.Parse(gitlabIssue.DueDate).ToUniversalTime()
                        : null,
                    LastModifiedAt = DateTime.Parse(gitlabIssue.UpdatedAt).ToUniversalTime()
                });
            }
            else
            {
                IList<LinkIssue>? links = await restService.GetIssueLinksAsync(projectId, gitlabIssue.Iid);
                IssueDescription gitlabIssueDescription = ParseDescription(gitlabIssue.Description);
                issues.Add(new Issue
                {
                    GitlabId = gitlabIssue.Id,
                    GitlabIid = gitlabIssue.Iid,
                    Title = gitlabIssue.Title,
                    Description = gitlabIssueDescription.Description,
                    Priority =
                        Enum.GetValues<Priority>().Length < gitlabIssueDescription.Priority &&
                        gitlabIssueDescription.Priority >= 0
                            ? Enum.GetValues<Priority>()[gitlabIssueDescription.Priority]
                            : Priority.Medium,
                    LinkedIssues = links?.Where(link =>
                            !link.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
                        .Select(link => new IssueLink
                        {
                            Type = LinkType.RelatesTo,
                            LinkedIssue =
                                new Issue
                                {
                                    GitlabId = link.Id.ToString(),
                                    GitlabIid = link.Iid.ToString(),
                                    Title = string.Empty
                                }
                        }).ToList() ?? [],
                    Release = links?.Where(link =>
                            link.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
                        .Select(link => new Release
                        {
                            GitlabId = link.Id.ToString(),
                            GitlabIid = link.Iid.ToString(),
                            Title = string.Empty
                        })
                        .FirstOrDefault(),
                    Milestone = new Milestone
                    {
                        Title = string.Empty,
                        GitlabId = gitlabIssue.Milestone?.Id,
                        GitlabIid = gitlabIssue.Milestone?.Iid
                    },
                    Labels = gitlabIssue.Labels?.Nodes?.Where(labelNode =>
                            labelNode != null &&
                            !labelNode.Title.StartsWith(AppearanceMarker, StringComparison.OrdinalIgnoreCase))
                        .Select(labelNode => new Label
                        {
                            Title = string.Empty,
                            Color = string.Empty,
                            TextColor = string.Empty,
                            GitlabId = labelNode!.Id
                        }).ToList() ?? [],
                    Vehicle = CreateVehicle(gitlabIssueDescription, gitlabIssue),
                    State = MapIssueState(gitlabIssue.State),
                    LastModifiedAt = DateTime.Parse(gitlabIssue.UpdatedAt).ToUniversalTime()
                });
            }
        }
    }

    private async Task ProcessRemainingIssuesAsync(int projectId,
        List<IGetFurtherIssues_Project_Issues_Nodes> gitlabFurtherIssues,
        IList<Release> releases, IList<Issue> issues)
    {
        foreach (IGetFurtherIssues_Project_Issues_Nodes gitlabIssue in gitlabFurtherIssues)
        {
            if (gitlabIssue.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
            {
                releases.Add(new Release
                {
                    GitlabId = gitlabIssue.Id,
                    GitlabIid = gitlabIssue.Iid,
                    Title = gitlabIssue.Title,
                    Notes = gitlabIssue.Description,
                    State = MapReleaseState(gitlabIssue.State),
                    Date = gitlabIssue.DueDate != null
                        ? DateTime.Parse(gitlabIssue.DueDate).ToUniversalTime()
                        : null,
                    LastModifiedAt = DateTime.Parse(gitlabIssue.UpdatedAt).ToUniversalTime()
                });
            }
            else
            {
                IList<LinkIssue>? links = await restService.GetIssueLinksAsync(projectId, gitlabIssue.Iid);
                IssueDescription gitlabIssueDescription = ParseDescription(gitlabIssue.Description);
                issues.Add(new Issue
                {
                    GitlabId = gitlabIssue.Id,
                    GitlabIid = gitlabIssue.Iid,
                    Title = gitlabIssue.Title,
                    Description = gitlabIssueDescription.Description,
                    Priority =
                        Enum.GetValues<Priority>().Length < gitlabIssueDescription.Priority &&
                        gitlabIssueDescription.Priority >= 0
                            ? Enum.GetValues<Priority>()[gitlabIssueDescription.Priority]
                            : Priority.Medium,
                    LinkedIssues = links?.Where(link =>
                            !link.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
                        .Select(link => new IssueLink
                        {
                            Type = LinkType.RelatesTo,
                            LinkedIssue =
                                new Issue
                                {
                                    GitlabId = link.Id.ToString(),
                                    GitlabIid = link.Iid.ToString(),
                                    Title = string.Empty
                                }
                        }).ToList() ?? [],
                    Release = links?.Where(link =>
                            link.Title.Contains(ReleaseTitle, StringComparison.InvariantCultureIgnoreCase))
                        .Select(link => new Release
                        {
                            GitlabId = link.Id.ToString(),
                            GitlabIid = link.Iid.ToString(),
                            Title = string.Empty
                        })
                        .FirstOrDefault(),
                    Milestone = new Milestone
                    {
                        Title = string.Empty,
                        GitlabId = gitlabIssue.Milestone?.Id,
                        GitlabIid = gitlabIssue.Milestone?.Iid
                    },
                    Labels = gitlabIssue.Labels?.Nodes?.Where(labelNode =>
                            labelNode != null &&
                            !labelNode.Title.StartsWith(AppearanceMarker, StringComparison.OrdinalIgnoreCase))
                        .Select(labelNode => new Label
                        {
                            Title = string.Empty,
                            Color = string.Empty,
                            TextColor = string.Empty,
                            GitlabId = labelNode!.Id
                        }).ToList() ?? [],
                    Vehicle = CreateVehicle(gitlabIssueDescription, gitlabIssue),
                    State = MapIssueState(gitlabIssue.State),
                    LastModifiedAt = DateTime.Parse(gitlabIssue.UpdatedAt).ToUniversalTime()
                });
            }
        }
    }

    private static Vehicle? CreateVehicle(IssueDescription gitlabIssueDescription,
        IGetAll_Project_Issues_Nodes gitlabIssue)
    {
        return !string.IsNullOrWhiteSpace(gitlabIssueDescription.EngineColor) ||
               gitlabIssueDescription.Translations.Any()
            ? new Vehicle
            {
                EngineColor = ParseEngineColor(gitlabIssueDescription.EngineColor),
                Translations =
                    gitlabIssueDescription.Translations.Where(translation =>
                        !string.IsNullOrWhiteSpace(translation.Text) &&
                        !string.IsNullOrWhiteSpace(translation.Country)).ToList(),
                Appearances = gitlabIssue.Labels?.Nodes?.Where(labelNode =>
                        labelNode != null &&
                        labelNode.Title.StartsWith(AppearanceMarker, StringComparison.OrdinalIgnoreCase))
                    .Select(labelNode => new Appearance
                    {
                        Title = string.Empty,
                        Color = string.Empty,
                        TextColor = string.Empty,
                        GitlabId = labelNode!.Id
                    }).ToList() ?? [],
            }
            : null;
    }

    private static Vehicle? CreateVehicle(IssueDescription gitlabIssueDescription,
        IGetFurtherIssues_Project_Issues_Nodes gitlabIssue)
    {
        return !string.IsNullOrWhiteSpace(gitlabIssueDescription.EngineColor) ||
               gitlabIssueDescription.Translations.Any()
            ? new Vehicle
            {
                EngineColor = ParseEngineColor(gitlabIssueDescription.EngineColor),
                Translations =
                    gitlabIssueDescription.Translations.Where(translation =>
                        !string.IsNullOrWhiteSpace(translation.Text) &&
                        !string.IsNullOrWhiteSpace(translation.Country)).ToList(),
                Appearances = gitlabIssue.Labels?.Nodes?.Where(labelNode =>
                        labelNode != null &&
                        labelNode.Title.StartsWith(AppearanceMarker, StringComparison.OrdinalIgnoreCase))
                    .Select(labelNode => new Appearance
                    {
                        Title = string.Empty,
                        Color = string.Empty,
                        TextColor = string.Empty,
                        GitlabId = labelNode!.Id
                    }).ToList() ?? [],
            }
            : null;
    }

    private static EngineColor ParseEngineColor(string? engineColor)
    {
        if (!string.IsNullOrWhiteSpace(engineColor))
        {
            return engineColor.Equals("-")
                ? EngineColor.Unknown
                : Enum.Parse<EngineColor>(string.Concat(engineColor[0].ToString().ToUpper(), engineColor.AsSpan(1)));
        }
        return EngineColor.Unknown;
    }

    private async Task<List<IGetFurtherIssues_Project_Issues_Nodes>> LoadAllRemainingIssues(
        IGetAll_Project_Issues_PageInfo? currentPageInfo, CancellationToken cancellationToken)
    {
        List<IGetFurtherIssues_Project_Issues_Nodes> gitlabFurtherIssues = [];

        if (HasNextPage(currentPageInfo))
        {
            IGetFurtherIssues_Project_Issues? nextIssuesResult =
                await graphQlService.GetFurtherIssuesAsync(currentPageInfo!.EndCursor!, cancellationToken);

            gitlabFurtherIssues.AddRange(nextIssuesResult?.Nodes?.Where(issue => issue != null)
                .Cast<IGetFurtherIssues_Project_Issues_Nodes>() ?? []);

            while (HasNextPage(nextIssuesResult))
            {
                nextIssuesResult =
                    await graphQlService.GetFurtherIssuesAsync(nextIssuesResult!.PageInfo.EndCursor!,
                        cancellationToken);
                gitlabFurtherIssues.AddRange(nextIssuesResult?.Nodes?.Where(issue => issue != null)
                    .Cast<IGetFurtherIssues_Project_Issues_Nodes>() ?? []);
            }
        }

        return gitlabFurtherIssues;
    }

    private static bool HasNextPage(IGetAll_Project_Issues_PageInfo? currentPageInfo)
    {
        return (currentPageInfo?.HasNextPage ?? false) && !string.IsNullOrEmpty(currentPageInfo.EndCursor);
    }

    private static bool HasNextPage(IGetFurtherIssues_Project_Issues? nextIssuesResult)
    {
        return (nextIssuesResult?.PageInfo.HasNextPage ?? false) &&
               !string.IsNullOrEmpty(nextIssuesResult.PageInfo.EndCursor);
    }

    private static ReleaseState MapReleaseState(IssueState issueState) =>
        issueState switch
        {
            IssueState.Opened => ReleaseState.Open,
            IssueState.Closed => ReleaseState.Finished,
            _ => ReleaseState.Planned
        };

    private static Domain.Issues.IssueState MapIssueState(IssueState issueState) =>
        issueState switch
        {
            IssueState.Opened => Domain.Issues.IssueState.Open,
            IssueState.Closed => Domain.Issues.IssueState.Closed,
            _ => Domain.Issues.IssueState.Closed
        };

    private static IssueDescription ParseDescription(string? description)
    {
        IssueDescription issueDescription = new IssueDescription();
        if (description != null)
        {
            int fieldsBeginning = description.IndexOf('{');
            if (fieldsBeginning != -1)
            {
                string fieldsText = description[fieldsBeginning..];
                IssueDescription? parsedDescription = JsonSerializer.Deserialize<IssueDescription>(fieldsText);

                if (parsedDescription != null)
                {
                    issueDescription = parsedDescription;
                }

                if (!string.IsNullOrWhiteSpace(description[..fieldsBeginning]))
                {
                    issueDescription.Description = description[..fieldsBeginning] + "\n" + issueDescription.Description;
                }
            }
            else
            {
                issueDescription.Description = description;
            }
        }

        return issueDescription;
    }
}
