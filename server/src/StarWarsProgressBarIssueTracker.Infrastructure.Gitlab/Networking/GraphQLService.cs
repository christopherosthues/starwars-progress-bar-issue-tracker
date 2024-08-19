using Microsoft.Extensions.Options;
using StarWarsProgressBarIssueTracker.Domain.Labels;
using StarWarsProgressBarIssueTracker.Domain.Releases;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Configuration;
using StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL;
using StrawberryShake;
using IssueState = StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.GraphQL.IssueState;

namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking;

public class GraphQLService(
    IOptions<GitlabConfiguration> configuration,
    IGitlabClient client)
{
    private readonly string _projectId = configuration.Value.ProjectPath ?? throw new ArgumentException("Project path must not be null!");

    public async Task<IGetAll_Project?> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await client.GetAll.ExecuteAsync(_projectId, cancellationToken);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetFurtherIssues_Project_Issues?> GetFurtherIssuesAsync(string afterId,
        CancellationToken cancellationToken)
    {
        var result = await client.GetFurtherIssues.ExecuteAsync(_projectId, afterId, cancellationToken);
        result.EnsureNoErrors();

        return result.Data?.Project?.Issues;
    }

    public async Task<ICreateLabel_LabelCreate?> CreateLabel(Label label, CancellationToken token)
    {
        var createLabelInput = new LabelCreateInput
        {
            Color = label.Color,
            Description = label.Description,
            Title = label.Title,
            ProjectPath = _projectId
        };
        var result = await client.CreateLabel.ExecuteAsync(createLabelInput, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.LabelCreate;
    }

    public async Task<IGetLabels_Project?> GetAllLabels(CancellationToken token)
    {
        var result = await client.GetLabels.ExecuteAsync(_projectId, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetLabel_Project?> GetLabel(string labelTitle, CancellationToken token)
    {
        var result = await client.GetLabel.ExecuteAsync(_projectId, labelTitle, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetMilestones_Project?> GetAllMilestones(CancellationToken token)
    {
        var result = await client.GetMilestones.ExecuteAsync(_projectId, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetMilestoneResult?> GetMilestone(string id, IReadOnlyList<string> milestoneTitle, CancellationToken token)
    {
        var result = await client.GetMilestone.ExecuteAsync(_projectId, id, milestoneTitle, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data;
    }

    public async Task<IGetReleases_Project_Issues?> GetReleases(CancellationToken cancellationToken)
    {
        var result = await client.GetReleases.ExecuteAsync(_projectId, cancellationToken).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project?.Issues;
    }

    public async Task<IGetRelease_Issue?> GetRelease(string id, CancellationToken token)
    {
        var result = await client.GetRelease.ExecuteAsync(id, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Issue;
    }

    public async Task<IGetReleaseIssues_Project?> GetReleaseIssues(IReadOnlyList<string> iids, CancellationToken token)
    {
        var result = await client.GetReleaseIssues.ExecuteAsync(_projectId, iids, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<ICreateRelease_CreateIssue?> CreateRelease(Release release, CancellationToken token)
    {
        var createReleaseInput = new CreateIssueInput
        {
            Title = release.Title,
            ProjectPath = _projectId,
            Type = IssueType.Issue,
        };
        var result = await client.CreateRelease.ExecuteAsync(createReleaseInput, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.CreateIssue;
    }

    public async Task<IGetInitialIssues_Project?> GetInitialIssues(CancellationToken token)
    {
        var result = await client.GetInitialIssues.ExecuteAsync(_projectId, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetNextIssues_Project_Issues?> GetNextIssues(string afterId, IssueState issueState, CancellationToken token)
    {
        var state = issueState == IssueState.Opened ? IssuableState.Opened : IssuableState.Closed;
        var result = await client.GetNextIssues.ExecuteAsync(_projectId, afterId, state, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project?.Issues;
    }

    public async Task<IGetIssue_Issue?> GetIssue(string id, CancellationToken token)
    {
        var result = await client.GetIssue.ExecuteAsync(id, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Issue;
    }

    public async Task<IGetEditIssue_Project?> GetEditIssue(string iid, CancellationToken token)
    {
        var result = await client.GetEditIssue.ExecuteAsync(iid, _projectId, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }

    public async Task<IGetProject_Project?> GetProject(CancellationToken token)
    {
        var result = await client.GetProject.ExecuteAsync(_projectId, token).ConfigureAwait(false);
        result.EnsureNoErrors();

        return result.Data?.Project;
    }
}
